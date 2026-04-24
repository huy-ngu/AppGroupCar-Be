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
        IBaseRepository<Shipment> _shipmentRepo,
        IBaseRepository<ShipmentRequest> _shipmentRequestRepo
        ) : IShipmentOfferService
    {
        public async Task AcceptAsync(Guid shipmentOfferId, Guid UserId)
        {
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(shipmentOfferId) ?? throw new AppException(404, "Không tìm thấy bản ghi");

            var shipmentRequest = await _shipmentRequestRepo.GetByIdAsync(shipmentOffer.RequestId) ?? throw new AppException(404, "Không tìm thấy bản ghi");

            if (UserId != shipmentRequest.CustomerId)
            {
                throw new AppException(403, "Bạn không được phép sửa bản ghi này");
            }
            shipmentOffer.Status = ShipmentOfferStatus.Accepted;
            _shipmentOfferRepo.Update(shipmentOffer);

            var newShipment = new Shipment
            {
                Id = Guid.NewGuid(),
                ShipmentOfferId = shipmentOffer.Id,
                PaymentStatus = PaymentStatus.Unpaid,
                ShipmentStatus = ShipmentStatus.Created,
                Price = shipmentOffer.ProposedPrice
            };
            await _shipmentRepo.AddAsync(newShipment);

            await _shipmentOfferRepo.SaveChangesAsync();
        }

        public async Task DriverAcceptAsync(Guid driverId, Guid shipmentOfferId)
        {
            var offer = await _offerRepo.GetByIdAsync(driverId) ?? throw new AppException(404, "Không tìm thấy bản ghi");
            if (driverId != offer.DriverId) { throw new AppException(403, "Bạn không được phép sửa bản ghi này"); }
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(shipmentOfferId) ?? throw new AppException(404, "Không tìm thấy bản ghi");

            shipmentOffer.Status = ShipmentOfferStatus.Accepted;
            _shipmentOfferRepo.Update(shipmentOffer);

            var newShipment = new Shipment
            {
                Id = Guid.NewGuid(),
                ShipmentOfferId = shipmentOffer.Id,
                PaymentStatus = PaymentStatus.Unpaid,
                ShipmentStatus = ShipmentStatus.Created,
                Price = shipmentOffer.ProposedPrice

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
                OfferId = null
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

        public async Task<GetShipmentOfferDTO> CreateFromOfferAsync(Guid offerId, Guid requestId, decimal ProposedPrice)
        {
            var offer = await _offerRepo.GetByIdAsync(offerId) ?? throw new AppException(404, "Không tìm thấy bản ghi");

            var shipmentRequest = await _shipmentRequestRepo.GetByIdAsync(offerId) ?? throw new AppException(404, "Không tìm thấy Shipment Request");

            var newShipmentOffer = new ShipmentOffer
            {
                Id = Guid.NewGuid(),
                OfferId = offerId,
                RequestId = requestId,
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
                OfferId = newShipmentOffer.OfferId,
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

        public async Task<IEnumerable<GetDetailShipmentOfferDTO>> GetByRequestId(Guid ShipmentRequestId)
        {
            var shipmentOffers = await _shipmentOfferRepo.GetQueryable()
                                .AsNoTracking()
                                .Where(s => s.Id == ShipmentRequestId)
                                .Select(n => new GetDetailShipmentOfferDTO
                                {
                                    Id = n.Id,
                                    RequestId = n.RequestId,
                                    ProposedPrice = n.ProposedPrice,
                                    Status = n.Status,
                                    Driver = new DriverShipmentOfferDTO
                                    {
                                        DriverId = n.DriverId,
                                        DriverName = n.Driver.Name
                                    },
                                    Vehicle = new VehicleShipmentOfferDTO
                                    {
                                        VehicleId = n.VehicleId,
                                        VehicleBrand = n.Vehicle.Brand,
                                    },
                                }).ToListAsync();
            return shipmentOffers;
        }


    }
}
