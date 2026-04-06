using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

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

        public async Task AcceptAsync(Guid shipmentOfferId)
        {
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(shipmentOfferId);
            if (shipmentOffer == null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            shipmentOffer.Status = ShipmentOfferStatus.Accepted;
            _shipmentOfferRepo.Update(shipmentOffer);
            await _shipmentOfferRepo.SaveChangesAsync();
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

        public async Task<IEnumerable<ShipmentOffer>> GetAll()
        {
            var list = await _shipmentOfferRepo.GetAllAsync();
            return list;
        }

        public async Task<IEnumerable<GetShipmentOfferDTO>> GetByRequestId(Guid ShipmentRequestId)
        {
            var shipmentOffers = await _shipmentOfferRepo.GetQueryable()
                                .AsNoTracking()
                                .Where(s => s.Id == ShipmentRequestId)
                                .Select(n => new GetShipmentOfferDTO
                                {
                                    RequestId = n.RequestId,
                                    DriverId = n.DriverId,
                                    VehicleId = n.VehicleId,
                                    ProposedPrice = n.ProposedPrice,
                                }).ToListAsync();
            return shipmentOffers;
        }
    }
}
