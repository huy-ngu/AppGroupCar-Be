using AppGroupCar.Data;
using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.UserCompanyServices
{
    public class UCService : IUCService
    {
        private readonly IBaseRepository<CompanyUser> _companyRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly ApplicationDbContext _context;


        public UCService(
            IBaseRepository<CompanyUser> companyRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }

        public async Task<CUResponseDto> CreateAsync(CreateCURequestDto request)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == request.CompanyId && c.DeleteAt == null);

            if (company == null)
                throw new AppException(404, "Company not found");

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
                throw new AppException(404, "User not found");

            var existing = await _context.CompanyUsers
                .FirstOrDefaultAsync(cu => cu.CompanyId == request.CompanyId && cu.UserId == request.UserId);

            if (existing == null)
            {
                var entity = _mapper.Map<CompanyUser>(request);
                await _companyRepository.AddAsync(entity);
                await _companyRepository.SaveChangesAsync();

                return _mapper.Map<CUResponseDto>(entity);
            }

            existing.DeleteAt = null;
            existing.IsActive = true;
            existing.LastUpdateAt = DateTime.UtcNow;

            _companyRepository.Update(existing);
            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<CUResponseDto>(existing);
        }

        public async Task<bool> LeaveCompanyAsync(LeaveCompanyRequestDto request)
        {
            CompanyUser? companyUser = null;

            if (request.CompanyUserId != null)
            {
                companyUser = await _context.CompanyUsers
                    .FirstOrDefaultAsync(cu =>
                        cu.Id == request.CompanyUserId &&
                        cu.DeleteAt == null);
            }
            else if (request.UserId != null && request.CompanyId != null)
            {
                companyUser = await _context.CompanyUsers
                    .FirstOrDefaultAsync(cu =>
                        cu.UserId == request.UserId &&
                        cu.CompanyId == request.CompanyId &&
                        cu.DeleteAt == null);
            }
            if (companyUser == null)
                return false;

            companyUser.IsActive = false;
            companyUser.DeleteAt = DateTime.UtcNow;
            companyUser.LastUpdateAt = DateTime.UtcNow;

            _companyRepository.Update(companyUser);
            await _companyRepository.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<CUResponseDto>> GetUsersByCompanyAsync(
            UserCompanyparameters param,
            Guid companyId)
        {
            var query = _context.CompanyUsers
                .AsNoTracking()
                .Where(cu => cu.CompanyId == companyId && cu.DeleteAt == null);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(cu => cu.CreatedAt)
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return new PagedResult<CUResponseDto>
            {
                Items = _mapper.Map<List<CUResponseDto>>(items),
                TotalCount = totalCount,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize
            };
        }

        public async Task<CUResponseDto> GetActiveCompanyByUserAsync(
                Guid userId,
                Guid companyId,
                UpdateCUStatusRequestDto request,
                Guid? actorUserId = null,
                bool isSystemAction = false)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId && c.DeleteAt == null);

            if (company == null)
                throw new AppException(404, "Company not found");

            var companyUser = await _context.CompanyUsers
                .FirstOrDefaultAsync(cu =>
                    cu.UserId == userId &&
                    cu.CompanyId == companyId &&
                    cu.DeleteAt == null);

            if (companyUser == null)
                throw new AppException(404, "Company user not found");

            var targetUser = await _userManager.FindByIdAsync(userId.ToString());
            if (targetUser == null)
                throw new AppException(404, "User not found");

            var isTargetAdmin = await _userManager.IsInRoleAsync(targetUser, "ADMIN_COMPANY");
            var isTargetStaff = await _userManager.IsInRoleAsync(targetUser, "STAFF_COMPANY");

            if (isSystemAction)
            {
                if (isTargetAdmin && request.IsActive && company.Status != CompanyStatus.Approved)
                    throw new AppException(400, "Admin company can only be active when company is approved");
            }
            else
            {
                if (actorUserId == null)
                    throw new AppException(401, "Invalid token");

                if (actorUserId == userId)
                    throw new AppException(403, "Admin cannot update their own active status");

                var actorCompanyUser = await _context.CompanyUsers
                    .FirstOrDefaultAsync(cu =>
                        cu.UserId == actorUserId &&
                        cu.CompanyId == companyId &&
                        cu.DeleteAt == null &&
                        cu.IsActive);

                if (actorCompanyUser == null)
                    throw new AppException(403, "You are not active in this company");

                var actorUser = await _userManager.FindByIdAsync(actorUserId.ToString()!);
                if (actorUser == null)
                    throw new AppException(404, "User not found");

                var isActorAdmin = await _userManager.IsInRoleAsync(actorUser, "ADMIN_COMPANY");
                if (!isActorAdmin)
                    throw new AppException(403, "Only ADMIN_COMPANY can update active status");

                if (!isTargetStaff || isTargetAdmin)
                    throw new AppException(403, "ADMIN_COMPANY can only update STAFF_COMPANY active status");
            }

            companyUser.IsActive = request.IsActive;
            companyUser.LastUpdateAt = DateTime.UtcNow;

            _companyRepository.Update(companyUser);
            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<CUResponseDto>(companyUser);
        }


    }
}
