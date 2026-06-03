using AppGroupCar.Data;
using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyDriverResponse;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppGroupCar.Services.Bus.CompanyDriverServices
{
    public class CompanyDriverService : ICompanyDriverService
    {
        private readonly IBaseRepository<CompanyDriver> _repo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPhotoService _photoService;

        public CompanyDriverService(
            IBaseRepository<CompanyDriver> repo,
            IMapper mapper,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IPhotoService photoService)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _photoService = photoService;
        }

        public async Task<PagedResult<CompanyDriverResponseDto>> GetByCompanyAsync(
            CompanyDriverParameters param,
            Guid companyId)
        {
            var query = _repo.GetQueryable()
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.DeleteAt == null);

            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                query = query.Where(x =>
                    x.LicenseNumber.Contains(param.Search) ||
                    x.User.FullName.Contains(param.Search));
            }

            if (param.DriverVerificationStatus.HasValue)
            {
                query = query.Where(x => x.DriverVerificationStatus == param.DriverVerificationStatus.Value);
            }

            var totalCount = await query.CountAsync();

            var entities = await query
                .Include(x => x.User)
                .Include(x => x.Company)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return new PagedResult<CompanyDriverResponseDto>
            {
                Items = _mapper.Map<List<CompanyDriverResponseDto>>(entities),
                TotalCount = totalCount,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize
            };
        }

        public async Task<CompanyDriverDetailResponseDto?> GetByIdAsync(Guid id, Guid companyId)
        {
            var entity = await _repo.GetQueryable()
                .Include(x => x.Company)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            return entity == null ? null : _mapper.Map<CompanyDriverDetailResponseDto>(entity);
        }

        public async Task<CompanyDriverResponseDto> CreateAsync(Guid companyId, CreateCompanyDriverRequestDto dto)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId && c.DeleteAt == null);

            if (company == null)
                throw new AppException(404, "Company not found");

            string? licensePublicId = null;
            string? licenseDocumentUrl = null;

            try
            {
                if (dto.LicenseDocumentImage != null)
                {
                    var uploadResult = await _photoService.UploadPhotoAsync(dto.LicenseDocumentImage, "company-driver-licenses");
                    if (uploadResult == null || uploadResult.Error != null)
                        throw new AppException(400, "Upload license document failed: " + uploadResult?.Error?.Message);

                    licensePublicId = uploadResult.PublicId;
                    licenseDocumentUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                var user = await CreateDriverUserAsync(dto);

                var existingDriver = await _repo.GetQueryable()
                    .FirstOrDefaultAsync(x =>
                        x.CompanyId == companyId &&
                        x.UserId == user.Id);

                if (existingDriver != null)
                {
                    if (existingDriver.DeleteAt == null)
                        throw new AppException(400, "User is already a driver in this company");

                    existingDriver.DeleteAt = null;
                    existingDriver.LastUpdateAt = DateTime.UtcNow;
                    existingDriver.LicenseNumber = dto.LicenseNumber;
                    existingDriver.LicenseClass = dto.LicenseClass;
                    existingDriver.DriverVerificationStatus = dto.DriverVerificationStatus;
                    if (!string.IsNullOrWhiteSpace(licenseDocumentUrl))
                        existingDriver.LicenseDocumentUrl = licenseDocumentUrl;

                    _repo.Update(existingDriver);
                    await _repo.SaveChangesAsync();

                    return _mapper.Map<CompanyDriverResponseDto>(existingDriver);
                }

                var entity = _mapper.Map<CompanyDriver>(dto);
                entity.Id = Guid.NewGuid();
                entity.CompanyId = companyId;
                entity.UserId = user.Id;
                entity.LicenseDocumentUrl = licenseDocumentUrl;

                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                return _mapper.Map<CompanyDriverResponseDto>(entity);
            }
            catch
            {
                if (!string.IsNullOrEmpty(licensePublicId))
                    await _photoService.DeletePhotoAsync(licensePublicId);

                throw;
            }
        }

        public async Task<CompanyDriverResponseDto> UpdateAsync(Guid id, Guid companyId, UpdateCompanyDriverRequestDto dto)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Company driver not found");

            string? newLicensePublicId = null;
            string? newLicenseUrl = null;
            string? oldLicensePublicId = null;

            try
            {
                if (dto.LicenseDocumentImage != null)
                {
                    var uploadResult = await _photoService.UploadPhotoAsync(dto.LicenseDocumentImage, "company-driver-licenses");
                    if (uploadResult == null || uploadResult.Error != null)
                        throw new AppException(400, "Upload license document failed: " + uploadResult?.Error?.Message);

                    newLicensePublicId = uploadResult.PublicId;
                    newLicenseUrl = uploadResult.SecureUrl.AbsoluteUri;
                    oldLicensePublicId = _photoService.GetPublicIdFromUrl(entity.LicenseDocumentUrl ?? string.Empty);
                }

                _mapper.Map(dto, entity);

                if (!string.IsNullOrWhiteSpace(newLicenseUrl))
                    entity.LicenseDocumentUrl = newLicenseUrl;

                entity.LastUpdateAt = DateTime.UtcNow;

                _repo.Update(entity);
                await _repo.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(oldLicensePublicId))
                    await _photoService.DeletePhotoAsync(oldLicensePublicId);

                return _mapper.Map<CompanyDriverResponseDto>(entity);
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(newLicensePublicId))
                    await _photoService.DeletePhotoAsync(newLicensePublicId);

                throw;
            }
        }

        public async Task DeleteAsync(Guid id, Guid companyId)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Company driver not found");

            entity.DeleteAt = DateTime.UtcNow;
            entity.LastUpdateAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }

        private async Task<ApplicationUser> CreateDriverUserAsync(CreateCompanyDriverRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new AppException(400, "Email, password, and full name are required");
            }

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new AppException(400, "Email already exists");

            var newUser = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString(),
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(newUser, dto.Password);
            if (!createResult.Succeeded)
            {
                throw new AppException(
                    400,
                    "Create user failed",
                    new { errors = createResult.Errors.Select(e => e.Description) }
                );
            }

            await EnsureStaffCompanyRoleAsync(newUser);
            return newUser;
        }

        private async Task EnsureStaffCompanyRoleAsync(ApplicationUser user)
        {
            const string roleName = "STAFF_COMPANY";

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new AppException(404, "Role not found: " + roleName);

            if (await _userManager.IsInRoleAsync(user, roleName))
                return;

            var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
            {
                throw new AppException(
                    400,
                    "Add role failed",
                    new { errors = addRoleResult.Errors.Select(e => e.Description) }
                );
            }
        }
    }
}
