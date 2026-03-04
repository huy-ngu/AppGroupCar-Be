using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Services.ShipmentRequestServices
{
    public class ShipmentRequestServices : IShipmentRequestServices
    {
        private readonly IBaseRepository<ShipmentRequest> _repo;
        private readonly IMapper _mapper;
        public ShipmentRequestServices(IBaseRepository<ShipmentRequest> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<ShipmentRequest> CreateAsync(Guid userId, CreateShipmentRequestDTO request)
        {
            var shipmentRequest = _mapper.Map<ShipmentRequest>(request);
            shipmentRequest.CustomerId = userId;

            await _repo.AddAsync(shipmentRequest);
            await _repo.SaveChangesAsync();
            return shipmentRequest;
        }

        public async Task DeleteAsync(Guid id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null && data.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            _repo.Delete(data);
            await _repo.SaveChangesAsync();
        }

        public async Task<ShipmentRequest> GetByIdAsync(Guid id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null || data?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            //if (data.DeleteAt != null)
            //{
            //    throw new AppException(404, "Không tìm thấy bản ghi");
            //}
            return data;
        }

        public async Task UpdateAsync(Guid id, UpdateShipmentRequestDTO request)
        {
            var exitShipmentRequest = await _repo.GetByIdAsync(id);
            if (exitShipmentRequest == null || exitShipmentRequest?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            _mapper.Map(request, exitShipmentRequest);
            _repo.Update(exitShipmentRequest);
            await _repo.SaveChangesAsync();
            return;
        }
    }
}
