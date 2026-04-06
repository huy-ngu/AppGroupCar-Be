using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.ShipmentOfferServices
{
    public class ShipmentOfferService(
        IBaseRepository<ShipmentOffer> _shipmentOfferRepo,
        IBaseRepository<Offer> _offerRepo,
        IBaseRepository<Shipment> _shipmentRepo
        ) : IShipmentOfferService
    {
        public async Task AcceptAsync(Guid shipmentOfferId)
        {
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(shipmentOfferId) ?? throw new AppException(404, "Không tìm thấy bản ghi");


            shipmentOffer.Status = ShipmentOfferStatus.Accepted;
            _shipmentOfferRepo.Update(shipmentOffer);

            var newShipment = new Shipment
            {
                Id = Guid.NewGuid(),
                ShipmentOfferId = shipmentOffer.Id,
                PaymentStatus = PaymentStatus.Unpaid,
                ShipmentStatus = ShipmentStatus.Created
            };
            await _shipmentRepo.AddAsync(newShipment);

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
                Id = newShipmentOffer.Id,
                RequestId = request.RequestId,
                DriverId = driverId,
                VehicleId = request.VehicleId,
                ProposedPrice = request.ProposedPrice
            };

            return response;
        }

        public async Task<GetShipmentOfferDTO> CreateFromOfferAsync(Guid offerId, decimal ProposedPrice)
        {
            var offer = await _offerRepo.GetByIdAsync(offerId) ?? throw new AppException(404, "Không tìm thấy bản ghi");

            var newShipmentOffer = new ShipmentOffer
            {
                Id = Guid.NewGuid(),
                OfferId = offerId,
                VehicleId = offer.VehicleId,
                DriverId = offer.DriverId,
                ProposedPrice = ProposedPrice,
                Status = ShipmentOfferStatus.Pending
            };
            await _shipmentOfferRepo.AddAsync(newShipmentOffer);

            await _shipmentOfferRepo.SaveChangesAsync();

            var response = new GetShipmentOfferDTO
            {
                Id = newShipmentOffer.Id,
                RequestId = newShipmentOffer.RequestId,
                DriverId = newShipmentOffer.DriverId,
                VehicleId = newShipmentOffer.VehicleId,
                ProposedPrice = newShipmentOffer.ProposedPrice
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
                                    Id = n.Id,
                                    RequestId = n.RequestId,
                                    DriverId = n.DriverId,
                                    VehicleId = n.VehicleId,
                                    ProposedPrice = n.ProposedPrice,
                                }).ToListAsync();
            return shipmentOffers;
        }
    }
}
