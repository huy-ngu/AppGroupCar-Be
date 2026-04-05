using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.Market.ShipmentServices
{
    public class ShipmentService : IShipmentService
    {
        private readonly IBaseRepository<Shipment> _shipmentRepo;
        private readonly IBaseRepository<ShipmentRequest> _shipmentRequestRepo;
        private readonly IBaseRepository<ShipmentOffer> _shipmentOfferRepo;


        public ShipmentService(IBaseRepository<Shipment> shipmentRepo, IBaseRepository<ShipmentRequest> shipmentRequestRepo, IBaseRepository<ShipmentOffer> shipmentOfferRepo)
        {
            _shipmentRepo = shipmentRepo;
            _shipmentRequestRepo = shipmentRequestRepo;
            _shipmentOfferRepo = shipmentOfferRepo;
        }
        public Task CreateFromOffer()
        {
            throw new NotImplementedException();
        }

        public async Task<GetShipmentDTO> CreateFromShipmentOffer(CreateShipmentFromShipmentOfferDTO request)
        {
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(request.ShipmentOfferId);
            if (shipmentOffer == null)
            {
                throw new AppException(404, "Không tìm thấy shipmentOffer");
            }

            var shipmentRequest = await _shipmentRequestRepo.GetByIdAsync(request.ShipmentRequestId);
            if (shipmentRequest == null)
            {
                throw new AppException(404, "Không tìm thấy shipmentRequest");
            }

            var newShipment = new Shipment
            {
                Id = Guid.NewGuid(),
                ShipmentOfferId = shipmentOffer.Id,
                DropoffLocationId = shipmentRequest.DropoffLocationId,
                PickupLocationId = shipmentRequest.PickupLocationId,
                Weight = shipmentRequest.Weight,
                Volume = shipmentRequest.Volume,
                HandlingNote = shipmentRequest.HandlingNote,
                ImageUrl = shipmentRequest.ImageUrl,
                Description = shipmentRequest.Description,
                IsFragile = shipmentRequest.IsFragile,
                ShipmentStatus = ShipmentStatus.Created,
                PaymentStatus = PaymentStatus.Unpaid,
                DeliveryDate = shipmentRequest.DeliveryDate,
            };

            await _shipmentRepo.AddAsync(newShipment);
            await _shipmentRepo.SaveChangesAsync();

            GetShipmentDTO response = new GetShipmentDTO
            {
                Id = newShipment.Id,
                ShipmentOfferId = shipmentOffer.Id,
                DropoffLocationId = shipmentRequest.DropoffLocationId,
                PickupLocationId = shipmentRequest.PickupLocationId,
                Weight = shipmentRequest.Weight,
                Volume = shipmentRequest.Volume,
                HandlingNote = shipmentRequest.HandlingNote,
                ImageUrl = shipmentRequest.ImageUrl,
                Description = shipmentRequest.Description,
                IsFragile = shipmentRequest.IsFragile,
                ShipmentStatus = ShipmentStatus.Created,
                PaymentStatus = PaymentStatus.Unpaid,
                DeliveryDate = shipmentRequest.DeliveryDate,
            };

            return response;
        }
    }
}
