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
using CloudinaryDotNet.Actions;
using AppGroupCar.Data;
using AppGroupCar.DTO.Filters.Bus;
using AppGroupCar.Services.Bus.UserCompanyServices;
using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;

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
                    // Lọc theo CompanyName
                    (string.IsNullOrEmpty(param.CompanyName)
                        || c.CompanyName.Contains(param.CompanyName)) &&

                    // Lọc theo Email
                    (string.IsNullOrEmpty(param.Email)
                        || c.Email.Contains(param.Email)) &&

                    // Chỉ lấy Company chưa bị xóa
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
                    company.Status = 0;

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

            var previousStatus = company.Status;

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

            if ((CompanyStatus)previousStatus == CompanyStatus.Rejected)
            {
                company.Status = (int)CompanyStatus.Pending;
                company.ApprovedAt = null;
            }

            _companyRepository.Update(company);
            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<CompanyResponseDto>(company);
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
                var user = await _userManager.FindByEmailAsync(company.Email);

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
                        UserName = $"{company.CompanyName}_Admin",
                        FullName = $"{company.CompanyName}_Business",
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
                var user = await _userManager.FindByEmailAsync(company.Email);

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
                var user = await _userManager.FindByEmailAsync(company.Email);

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
    }
}
