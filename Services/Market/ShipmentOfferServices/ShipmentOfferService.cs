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

        public async Task DriverAcceptAsync(Guid driverId, Guid shipmentOfferId, Guid offerId)
        {
            var offer = await _offerRepo.GetByIdAsync(offerId) ?? throw new AppException(404, "Không tìm thấy offer");
            if (driverId != offer.DriverId) { throw new AppException(403, "Bạn không được phép sửa bản ghi này"); }
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(shipmentOfferId) ?? throw new AppException(404, "Không tìm thấy shipment offer");

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
                OfferId = request.OfferId
            };
            await _shipmentOfferRepo.AddAsync(newShipmentOffer);

            await _shipmentOfferRepo.SaveChangesAsync();

            var response = new GetShipmentOfferDTO
            {
                Id = newShipmentOffer.Id,
                RequestId = request.RequestId,
                DriverId = driverId,
                VehicleId = request.VehicleId,
                OfferId = request.OfferId,
                ProposedPrice = request.ProposedPrice
            };

            return response;
        }

        public async Task<GetShipmentOfferDTO> CreateFromOfferAsync(CreateShipmentOfferFromOfferRequestDTO request)
        {
            var offer = await _offerRepo.GetByIdAsync(request.OfferId) ?? throw new AppException(404, "Không tìm thấy shipment offer");

            var shipmentRequest = await _shipmentRequestRepo.GetByIdAsync(request.RequestId) ?? throw new AppException(404, "Không tìm thấy Shipment Request");

            var newShipmentOffer = new ShipmentOffer
            {
                Id = Guid.NewGuid(),
                OfferId = request.OfferId,
                RequestId = request.RequestId,
                VehicleId = offer.VehicleId,
                DriverId = offer.DriverId,
                ProposedPrice = request.ProposedPrice,
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

        public async Task<IEnumerable<GetPendingShipmentOfferDTO>> GetPendingByOfferIdAsync(Guid driverId, Guid offerId)
        {
            if (driverId == Guid.Empty || offerId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var offer = await _offerRepo.GetByIdAsync(offerId)
                ?? throw new AppException(404, "Không tìm thấy offer");

            if (offer.DriverId != driverId)
            {
                throw new AppException(403, "Bạn không được phép xem danh sách này");
            }

            var shipmentOffers = await _shipmentOfferRepo.GetQueryable()
                .AsNoTracking()
                .Where(s => s.DeleteAt == null &&
                            s.OfferId == offerId &&
                            s.Status == ShipmentOfferStatus.Pending)
                .Select(n => new GetPendingShipmentOfferDTO
                {
                    Id = n.Id,
                    OfferId = n.OfferId,
                    RequestId = n.RequestId,
                    ProposedPrice = n.ProposedPrice,
                    Status = n.Status,
                    CreatedAt = n.CreatedAt,
                    ShipmentRequest = new ShipmentRequestInfoDTO
                    {
                        Id = n.Request.Id,
                        CreatedAt = n.Request.CreatedAt,
                        LastUpdateAt = n.Request.LastUpdateAt,
                        DeleteAt = n.Request.DeleteAt,
                        CustomerId = n.Request.CustomerId,
                        PickupLocationId = n.Request.PickupLocationId,
                        DropoffLocationId = n.Request.DropoffLocationId,
                        DeliveryDate = n.Request.DeliveryDate,
                        Weight = n.Request.Weight,
                        Volume = n.Request.Volume,
                        Description = n.Request.Description,
                        IsFragile = n.Request.IsFragile,
                        HandlingNote = n.Request.HandlingNote,
                        Status = n.Request.Status,
                        ImageUrl = n.Request.ImageUrl
                    },
                })
                .ToListAsync();

            return shipmentOffers;
        }

        public async Task<IEnumerable<GetDetailShipmentOfferDTO>> GetByRequestId(Guid ShipmentRequestId)
        {
            var shipmentOffers = await _shipmentOfferRepo.GetQueryable()
                                .AsNoTracking()
                                .Where(s => s.RequestId == ShipmentRequestId)
                                .Select(n => new GetDetailShipmentOfferDTO
                                {
                                    Id = n.Id,
                                    RequestId = n.RequestId,
                                    ProposedPrice = n.ProposedPrice,
                                    Status = n.Status,
                                    OfferId = n.OfferId,
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

        public async Task CancelAsync(Guid driverId, Guid shipmentOfferId)
        {
            var shipmentOffer = await _shipmentOfferRepo.GetByIdAsync(shipmentOfferId)
                ?? throw new AppException(404, "Không tìm thấy bản ghi");

            if (shipmentOffer.DeleteAt != null || shipmentOffer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền hủy");
            }

            if (shipmentOffer.Status != ShipmentOfferStatus.Pending)
            {
                throw new AppException(400, "Chỉ có thể hủy ShipmentOffer ở trạng thái Pending");
            }

            shipmentOffer.Status = ShipmentOfferStatus.Cancelled;
            _shipmentOfferRepo.Update(shipmentOffer);

            var shipments = await _shipmentRepo.GetQueryable()
                .Where(s => s.ShipmentOfferId == shipmentOfferId && s.DeleteAt == null)
                .ToListAsync();

            foreach (var shipment in shipments)
            {
                shipment.ShipmentStatus = ShipmentStatus.Cancelled;
                _shipmentRepo.Update(shipment);
            }

            await _shipmentOfferRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetDetailShipmentOfferDTO>> GetMyShipmentOffersAsync(Guid driverId)
        {
            if (driverId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var shipmentOffers = await _shipmentOfferRepo.GetQueryable()
                                .AsNoTracking()
                                .Where(s => s.DriverId == driverId && s.DeleteAt == null)
                                .Select(n => new GetDetailShipmentOfferDTO
                                {
                                    Id = n.Id,
                                    RequestId = n.RequestId,
                                    ProposedPrice = n.ProposedPrice,
                                    Status = n.Status,
                                    CreatedAt = n.CreatedAt,
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
