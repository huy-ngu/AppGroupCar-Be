using AppGroupCar.DTO.RequestDTO.Bus.SeatLayoutRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatLayoutResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.SeatLayoutServices
{
    public class SeatLayoutService : ISeatLayoutService
    {
        private readonly IBaseRepository<VehicleSeatLayout> _repo;
        private readonly IMapper _mapper;

        public SeatLayoutService(
            IBaseRepository<VehicleSeatLayout> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<VehicleSeatLayoutResponseDto> CreateAsync(CreateVehicleSeatLayoutRequest dto)
        {
            var entity = _mapper.Map<VehicleSeatLayout>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<VehicleSeatLayoutResponseDto>(entity);
        }

        public async Task<IEnumerable<VehicleSeatLayoutResponseDto>> GetAllAsync()
        {
            var items = await _repo.GetQueryable()
                .Where(x => x.DeleteAt == null && x.IsActive)
                .OrderBy(x => x.SeatCapacity)
                .ThenBy(x => x.SeatLayoutName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VehicleSeatLayoutResponseDto>>(items);
        }
    }
}
