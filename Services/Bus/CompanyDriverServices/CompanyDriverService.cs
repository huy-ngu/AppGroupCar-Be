using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyDriverResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.CompanyDriverServices
{
    public class CompanyDriverService : ICompanyDriverService
    {
        private readonly IBaseRepository<CompanyDriver> _repo;
        private readonly IMapper _mapper;

        public CompanyDriverService(
            IBaseRepository<CompanyDriver> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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
                .OrderByDescending(x => x.CreatedAt)
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .Include(x => x.User)
                .Include(x => x.Company)
                .ToListAsync();

            return new PagedResult<CompanyDriverResponseDto>
            {
                Items = _mapper.Map<List<CompanyDriverResponseDto>>(entities),
                TotalCount = totalCount,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize
            };
        }

        public async Task<CompanyDriverResponseDto> CreateAsync(Guid companyId, CreateCompanyDriverRequestDto dto)
        {
            var existingDriver = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == companyId &&
                    x.UserId == dto.UserId &&
                    x.DeleteAt == null);

            if (existingDriver != null)
                throw new AppException(400, "User is already a driver in this company");

            var entity = _mapper.Map<CompanyDriver>(dto);
            entity.Id = Guid.NewGuid();
            entity.CompanyId = companyId;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<CompanyDriverResponseDto>(entity);
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

            _mapper.Map(dto, entity);

            _repo.Update(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<CompanyDriverResponseDto>(entity);
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

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
