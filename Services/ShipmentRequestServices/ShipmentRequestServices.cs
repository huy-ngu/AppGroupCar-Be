using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.ShipmentRequestServices
{
    public class ShipmentRequestServices : IShipmentRequestServices
    {
        private readonly IBaseRepository<ShipmentRequest> _repo;
        public ShipmentRequestServices(IBaseRepository<ShipmentRequest> repo)
        {
            _repo = repo;
        }
        public async Task<ShipmentRequest> CreateAsync(Guid userId, CreateShipmentRequestDTO request)
        {
            ShipmentRequest shipmentRequest = new ShipmentRequest
            {
                CustomerId = userId,
                PickupLocationId = request.PickupLocationId,
                DropoffLocationId = request.DropoffLocationId,
                Weight = request.Weight,
                Volume = request.Volume,
                Description = request.Description,
                IsFragile = request.IsFragile,
                IsPerishable = request.IsPerishable,
                RequiresCooler = request.RequiresCooler,
                AvoidSunLight = request.AvoidSunLight,
                HandlingNote = request.HandlingNote,
                CreatedAt = DateTime.UtcNow,
                DeleteAt = null,
                LastUpdateAt = null
            };
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
            if (data == null && data.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            return data;
        }


    }
}
