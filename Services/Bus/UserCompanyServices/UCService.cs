using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Data;
using Microsoft.EntityFrameworkCore;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;
using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using AppGroupCar.Exceptions;

namespace AppGroupCar.Services.Bus.UserCompanyServices
{
    public class UCService : IUCService
    {
            private readonly IBaseRepository<CompanyUser> _companyRepository;
            private readonly IMapper _mapper;
            private readonly UserManager<ApplicationUser>_userManager;
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

            public async Task<IEnumerable<CUResponseDto>> GetUsersByCompanyAsync(Guid companyId)
            {
                var companyUsers = await _context.CompanyUsers
                    .Where(cu => cu.CompanyId == companyId && cu.DeleteAt == null)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<CUResponseDto>>(companyUsers);
            }

            public async Task<CUResponseDto> GetActiveCompanyByUserAsync(
                Guid userId,
                Guid companyId,
                UpdateCUStatusRequestDto request)
            {
                var companyUser = await _context.CompanyUsers
                    .FirstOrDefaultAsync(cu =>
                        cu.UserId == userId &&
                        cu.CompanyId == companyId &&
                        cu.DeleteAt == null);

                if (companyUser == null)
                    throw new AppException(404, "Company user not found");

                companyUser.IsActive = request.IsActive;
                companyUser.LastUpdateAt = DateTime.UtcNow;

                _companyRepository.Update(companyUser);
                await _companyRepository.SaveChangesAsync();

                return _mapper.Map<CUResponseDto>(companyUser);
            }
    }
}
