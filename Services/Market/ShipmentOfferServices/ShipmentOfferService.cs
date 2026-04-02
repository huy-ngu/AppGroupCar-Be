using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.Market.ShipmentOfferServices
{
    public class ShipmentOfferService : IShipmentOfferService
    {
        private readonly IBaseRepository<ShipmentOffer> _shipmentOfferRepo;
        private readonly IBaseRepository<Shipment> _shipmentRepo;

        public ShipmentOfferService(IBaseRepository<ShipmentOffer> shipmentOfferRepo, IBaseRepository<Shipment> shipmentRepo)
        {
            _shipmentOfferRepo = shipmentOfferRepo;
            _shipmentRepo = shipmentRepo;
        }

        public async Task<GetShipmentOfferDTO> CreateAsync(Guid driverId, CreateShipmentOfferRequestDTO request)
        {
            var newShipmentOffer = new ShipmentOffer
            {
                Id = Guid.NewGuid(),
                DriverId = driverId,
                RequestId = request.RequestId,
                VehicleId = request.VehicleId,
                ProposedPrice = request.ProposedPrice,
            };
            await _shipmentOfferRepo.AddAsync(newShipmentOffer);

            await _shipmentOfferRepo.SaveChangesAsync();

            var response = new GetShipmentOfferDTO
            {
                RequestId = request.RequestId,
                DriverId = driverId,
                VehicleId = request.VehicleId,
                ProposedPrice = request.ProposedPrice
            };

            return response;
        }
    }
}
