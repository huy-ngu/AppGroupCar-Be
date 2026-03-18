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

namespace AppGroupCar.Services.Bus.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private readonly IBaseRepository<Company> _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        protected readonly ApplicationDbContext _context;

        public CompanyService(
            IBaseRepository<Company> companyRepository,
            IMapper mapper,
            IPhotoService photoService,
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _photoService = photoService;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<CompanyDetailResponseDto?> GetByIdAsync(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company == null)
                return null;

            return _mapper.Map<CompanyDetailResponseDto>(company);
        }

        public async Task<PagedResult<CompanyResponseDto>> GetCompaniesAsync(
            int pageNumber,
            int pageSize,
            string? keyword)
        {
            var result = await _companyRepository.FindAndFilter(
                predicate: c =>
                    c.DeleteAt == null &&
                    (string.IsNullOrEmpty(keyword) ||
                     c.CompanyName.Contains(keyword) ||
                     c.CompanyCode.Contains(keyword)),
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            return new PagedResult<CompanyResponseDto>
            {
                Items = _mapper.Map<IEnumerable<CompanyResponseDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
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

            _companyRepository.Delete(company);

            await _companyRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<CompanyResponseDto>> GetPendingCompaniesAsync(
    int pageNumber,
    int pageSize)
        {
            var result = await _companyRepository.FindAndFilter(
                predicate: c => c.DeleteAt == null && c.Status == 0,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            return new PagedResult<CompanyResponseDto>
            {
                Items = _mapper.Map<IEnumerable<CompanyResponseDto>>(result.Items),
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

            if (company.Status != CompanyStatus.Pending)
                throw new AppException(400, "Company already processed");

            if (dto.Status != CompanyStatus.Approved && dto.Status != CompanyStatus.Rejected && dto.Status != CompanyStatus.Suspended)
                throw new AppException(400, "Invalid status");

            company.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Description))
                company.Description = dto.Description;

            company.ApprovedAt = DateTime.UtcNow;

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
                    else
                    {
                        throw new AppException(400, "Email already exists");
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
                }
            }
            else if (dto.Status == CompanyStatus.Suspended) // Suspended
            {
                var user = await _userManager.FindByEmailAsync(company.Email);

                if (user != null && user.DeleteAt == null)
                {
                    await _userManager.DeleteAsync(user); // soft delete
                }
            }

            _companyRepository.Update(company);
            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<CompanyResponseDto>(company);
        }
    }
}
