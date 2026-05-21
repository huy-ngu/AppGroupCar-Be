using AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.DTO;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using AppGroupCar.Services.Mytoys.Photo;
using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Data;
using AppGroupCar.DTO.Filters.Bus;
using AppGroupCar.Services.Bus.UserCompanyServices;
using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using System.Globalization;
using System.Text;

namespace AppGroupCar.Services.Bus.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private readonly IBaseRepository<Company> _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUCService _ucService;
        protected readonly ApplicationDbContext _context;

        public CompanyService(
            IBaseRepository<Company> companyRepository,
            IMapper mapper,
            IPhotoService photoService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUCService ucService,
            ApplicationDbContext context)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _photoService = photoService;
            _userManager = userManager;
            _roleManager = roleManager;
            _ucService = ucService;
            _context = context;
        }

        public async Task<CompanyDetailResponseDto?> GetByIdAsync(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                return null;

            return _mapper.Map<CompanyDetailResponseDto>(company);
        }

        public async Task<PagedResult<CompanyResponseDto>> FilterAsync(CompanyParameters param)
        {
            var data = await _companyRepository.FindAndFilter(
                predicate: c =>
                    (
                        string.IsNullOrEmpty(param.Search) ||
                        c.CompanyName.Contains(param.Search) ||
                        ((c.Email ?? string.Empty).Contains(param.Search))
                    )
                    &&
                    (
                        param.CompanyStatus == null || c.Status == param.CompanyStatus
                    )
                    &&
                    c.DeleteAt == null,

                orderBy: q => q.OrderByDescending(c => c.CompanyName),

                pageNumber: param.PageNumber,
                pageSize: param.PageSize
            );

            // Mapping sang DTO
            var result = new PagedResult<CompanyResponseDto>
            {
                Items = _mapper.Map<IEnumerable<CompanyResponseDto>>(data.Items),
                TotalCount = data.TotalCount,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize
            };

            return result;
        }

        public async Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            var check = await _companyRepository.GetQueryable().Where(c => c.Email == dto.Email).FirstOrDefaultAsync();

            if (check != null)
            {
                throw new AppException(400, "Email đã tồn tại");
            }

            string? logoUrl = null;
            string? publicId = null;

            Company company = null!;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    company = _mapper.Map<Company>(dto);
                    company.Id = Guid.NewGuid();
                    company.Status = CompanyStatus.Pending;
                    company.CreatedAt = DateTime.UtcNow;
                    await _companyRepository.AddAsync(company);
                    await _companyRepository.SaveChangesAsync();

                    if (dto.Logo != null)
                    {
                        var uploadResult = await _photoService.UploadPhotoAsync(dto.Logo, "company-logos");

                        if (uploadResult.Error != null)
                            throw new AppException(400, "Upload logo failed: " + uploadResult.Error.Message);

                        logoUrl = uploadResult.SecureUrl.AbsoluteUri;
                        publicId = uploadResult.PublicId;

                        company.LogoUrl = logoUrl;

                        _companyRepository.Update(company);
                        await _companyRepository.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();

                    if (publicId != null)
                        await _photoService.DeletePhotoAsync(publicId);

                    throw;
                }
            });

            return _mapper.Map<CompanyResponseDto>(company);
        }

        public async Task<CompanyResponseDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto dto)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new AppException(404, "Company not found");

            if (company.Status == CompanyStatus.Approved)
            {
                await ApproveCompanyAsync(id, new ApproveCompanyRequestDto
                {
                    Status = CompanyStatus.Pending
                });

                company = await _companyRepository.GetByIdAsync(id);

                if (company == null)
                    throw new AppException(404, "Company not found");
            }

            if (company.Status != CompanyStatus.Pending && company.Status != CompanyStatus.Rejected)
                throw new AppException(400, "Chỉ công ty ở trạng thái Pending hoặc Rejected mới được cập nhật");

            _mapper.Map(dto, company);

            if (dto.Logo != null)
            {
                // Upload ảnh mới
                var uploadResult = await _photoService.UploadPhotoAsync(dto.Logo, "company-logos");

                if (uploadResult.Error != null)
                    throw new AppException(400, "Upload logo failed: " + uploadResult.Error.Message);

                // Xóa logo cũ nếu có
                if (!string.IsNullOrEmpty(company.LogoUrl))
                {
                    var publicId = _photoService.GetPublicIdFromUrl(company.LogoUrl);
                    await _photoService.DeletePhotoAsync(publicId);
                }

                company.LogoUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            // Sau khi công ty bị từ chối (Rejected = 2) chỉnh sửa hồ sơ,
            // hệ thống tự đưa về Pending để chờ duyệt lại.
            if (company.Status == CompanyStatus.Rejected)
            {
                company.Status = CompanyStatus.Pending;
                company.ApprovedAt = null;
            }

            _companyRepository.Update(company);
            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<CompanyResponseDto>(company);
        }

        public async Task<CompanyLoginResultDto> LoginCompanyAsync(CompanyLoginRequestDto dto)
        {
            var company = await _companyRepository
                .GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.DeleteAt == null
                    && c.Email != null
                    && c.Email == dto.Email
                    && c.BusinessLicenseNo == dto.BusinessLicenseNo);

            if (company == null)
                throw new AppException(401, "Email hoặc BusinessLicenseNo không đúng");

            var result = new CompanyLoginResultDto
            {
                CompanyId = company.Id,
                Status = company.Status,
                Description = company.Description
            };

            if (company.Status == CompanyStatus.Pending || company.Status == CompanyStatus.Rejected)
            {
                result.PendingUpdateData = new PendingCompanyUpdateDataDto
                {
                    CompanyId = company.Id,
                    CompanyName = company.CompanyName,
                    LogoUrl = company.LogoUrl,
                    Phone = company.Phone,
                    Address = company.Address,
                    ProvinceCode = company.ProvinceCode,
                    DistrictCode = company.DistrictCode,
                    BusinessLicenseNo = company.BusinessLicenseNo
                };
            }

            return result;
        }

        public async Task DeleteCompanyAsync(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new AppException(404, "Company not found");

            company.DeleteAt = DateTime.UtcNow;
            _companyRepository.Update(company);

            await _companyRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<CompanyResponseDto>> GetPendingCompaniesAsync(
            CompanyParameters param)
        {
            var result = await _companyRepository.FindAndFilter(
                predicate: c =>
                    c.DeleteAt == null &&
                    c.Status == CompanyStatus.Pending,

                orderBy: q => q
                    .OrderByDescending(x => x.CreatedAt),

                pageNumber: param.PageNumber,
                pageSize: param.PageSize
            );

            var items = result.Items.Select(c => new CompanyResponseDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Email = c.Email,
                Phone = c.Phone,
                LogoUrl = c.LogoUrl,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            });

            return new PagedResult<CompanyResponseDto>
            {
                Items = items,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<CompanyResponseDto> ApproveCompanyAsync(Guid id, ApproveCompanyRequestDto dto)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new AppException(404, "Company not found");

            if (company.Status != CompanyStatus.Pending
                && company.Status != CompanyStatus.Suspended
                && company.Status != CompanyStatus.Approved)
                throw new AppException(400, "Company already processed");

            if (dto.Status != CompanyStatus.Approved
                && dto.Status != CompanyStatus.Rejected
                && dto.Status != CompanyStatus.Suspended
                && dto.Status != CompanyStatus.Pending)
                throw new AppException(400, "Invalid status");

            company.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Description))
                company.Description = dto.Description;

            company.ApprovedAt = dto.Status == CompanyStatus.Pending ? null : DateTime.UtcNow;

            // xử lý user theo status
            if (dto.Status == CompanyStatus.Approved) // Approved
            {
                var user = await FindCompanyUserByEmailAsync(company.Email);

                if (user != null)
                {
                    // user tồn tại nhưng bị soft delete
                    if (user.DeleteAt != null)
                    {
                        user.DeleteAt = null;
                        await _userManager.UpdateAsync(user);
                    }
                }
                else
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = BuildCompanyAdminUserName(company.CompanyName),
                        FullName = $"{company.CompanyName} Business",
                        Email = company.Email,
                        PhoneNumber = company.Phone,
                        AvatarUrl = company.LogoUrl
                    };

                    var result = await _userManager.CreateAsync(newUser, company.BusinessLicenseNo);

                    if (!result.Succeeded)
                        throw new AppException(
                            400,
                            "Create user failed",
                            new { errors = result.Errors.Select(e => e.Description) }
                        );

                    var roleName = "ADMIN_COMPANY";

                    if (!await _roleManager.RoleExistsAsync(roleName))
                        throw new AppException(404, $"Role not found: {roleName}");

                    await _userManager.AddToRoleAsync(newUser, roleName);

                    user = newUser;
                }

                await _ucService.CreateAsync(new CreateCURequestDto
                {
                    CompanyId = company.Id,
                    UserId = user.Id
                });
            }
            else if (dto.Status == CompanyStatus.Suspended) // Suspended
            {
                var user = await FindCompanyUserByEmailAsync(company.Email);

                if (user != null && user.DeleteAt == null)
                {
                    var companyUser = await _context.CompanyUsers
                        .FirstOrDefaultAsync(cu => cu.CompanyId == company.Id && cu.UserId == user.Id);

                    if (companyUser != null)
                        await _ucService.GetActiveCompanyByUserAsync(
                            user.Id,
                            company.Id,
                            new UpdateCUStatusRequestDto { IsActive = false },
                            null,
                            true);
                }
            }
            else if (dto.Status == CompanyStatus.Pending || dto.Status == CompanyStatus.Rejected)
            {
                var user = await FindCompanyUserByEmailAsync(company.Email);

                if (user != null)
                {
                    var companyUser = await _context.CompanyUsers
                        .FirstOrDefaultAsync(cu => cu.CompanyId == company.Id && cu.UserId == user.Id);

                    if (companyUser != null)
                        await _ucService.GetActiveCompanyByUserAsync(
                            user.Id,
                            company.Id,
                            new UpdateCUStatusRequestDto { IsActive = false },
                            null,
                            true);
                }
            }

            _companyRepository.Update(company);
            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<CompanyResponseDto>(company);
        }

        private async Task<ApplicationUser?> FindCompanyUserByEmailAsync(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _userManager.FindByEmailAsync(email);
        }

        private static string BuildCompanyAdminUserName(string companyName)
        {
            var cleanedName = RemoveDiacritics(companyName);
            var safeChars = cleanedName
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.' || c == '@' || c == '+')
                .ToArray();

            var safeName = new string(safeChars);
            if (string.IsNullOrWhiteSpace(safeName))
                safeName = "company";

            return $"{safeName}_Admin_{Guid.NewGuid():N}";
        }

        private static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task LockCompanyAsync(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new AppException(404, "Company not found");

            company.Status = CompanyStatus.Suspended;
            _companyRepository.Update(company);

            await _companyRepository.SaveChangesAsync();
        }

        public async Task UnlockCompanyAsync(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new AppException(404, "Company not found");
            company.Status = CompanyStatus.Suspended;

            _companyRepository.Update(company);

            await _companyRepository.SaveChangesAsync();
        }
    }
}


