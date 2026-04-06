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

            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(request.ShipmentOfferId) ?? throw new AppException(404, "Không tìm thấy bản ghi");

            if (shipmentOffer.Status != ShipmentOfferStatus.Accepted)
            {
                throw new AppException(400, "Không được phép tạo!");
            }

            var shipmentRequest = await _shipmentRequestRepo.GetByIdAsync(request.ShipmentRequestId) ?? throw new AppException(404, "Không tìm thấy bản ghi");


            var newShipment = new Shipment
            {
                Id = Guid.NewGuid(),
                ShipmentOfferId = shipmentOffer.Id,
                ShipmentStatus = ShipmentStatus.Created,
                PaymentStatus = PaymentStatus.Unpaid,
            };

            await _shipmentRepo.AddAsync(newShipment);
            await _shipmentRepo.SaveChangesAsync();

            GetShipmentDTO response = new GetShipmentDTO
            {
                Id = newShipment.Id,
                ShipmentOfferId = shipmentOffer.Id,
                DeliveryDate = shipmentRequest.DeliveryDate,
                ShipmentStatus = ShipmentStatus.Created,
                PaymentStatus = PaymentStatus.Unpaid,
            };

            return response;
        }

        Task<GetShipmentDTO> IShipmentService.CreateFromOffer()
        {
            throw new NotImplementedException();
        }
    }
}
