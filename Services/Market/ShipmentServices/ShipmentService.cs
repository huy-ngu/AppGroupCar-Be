using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.ShipmentServices
{
    public class ShipmentService(
            IBaseRepository<Shipment> _shipmentRepo
        ) : IShipmentService
    {
        public async Task<GetShipmentDetailDTO> GetDetailByIdAsync(Guid userId, Guid shipmentId)
        {
            if (userId == Guid.Empty || shipmentId == Guid.Empty)
            {
                throw new AppException(400, "ID khong hop le");
            }

            var detail = await _shipmentRepo.GetQueryable()
                .AsNoTracking()
                .Where(s => s.Id == shipmentId && s.ShipmentOffer != null && s.ShipmentOffer.Request != null)
                .Select(s => new
                {
                    Shipment = s,
                    ShipmentOffer = s.ShipmentOffer!,
                    ShipmentRequest = s.ShipmentOffer!.Request
                })
                .FirstOrDefaultAsync();

            if (detail == null)
            {
                throw new AppException(404, "Khong tim thay ban ghi");
            }

            var isOwner = detail.ShipmentOffer.DriverId == userId ||
                          detail.ShipmentRequest.CustomerId == userId;
            if (!isOwner)
            {
                throw new AppException(403, "Ban khong duoc phep xem ban ghi nay");
            }

            return new GetShipmentDetailDTO
            {
                Shipment = new ShipmentInfoDTO
                {
                    Id = detail.Shipment.Id,
                    CreatedAt = detail.Shipment.CreatedAt,
                    LastUpdateAt = detail.Shipment.LastUpdateAt,
                    DeleteAt = detail.Shipment.DeleteAt,
                    ShipmentOfferId = detail.Shipment.ShipmentOfferId,
                    ShipmentStatus = detail.Shipment.ShipmentStatus,
                    PaymentStatus = detail.Shipment.PaymentStatus
                },
                ShipmentOffer = new ShipmentOfferInfoDTO
                {
                    Id = detail.ShipmentOffer.Id,
                    CreatedAt = detail.ShipmentOffer.CreatedAt,
                    LastUpdateAt = detail.ShipmentOffer.LastUpdateAt,
                    DeleteAt = detail.ShipmentOffer.DeleteAt,
                    RequestId = detail.ShipmentOffer.RequestId,
                    OfferId = detail.ShipmentOffer.OfferId,
                    DriverId = detail.ShipmentOffer.DriverId,
                    VehicleId = detail.ShipmentOffer.VehicleId,
                    ProposedPrice = detail.ShipmentOffer.ProposedPrice,
                    Status = detail.ShipmentOffer.Status
                },
                ShipmentRequest = new ShipmentRequestInfoDTO
                {
                    Id = detail.ShipmentRequest.Id,
                    CreatedAt = detail.ShipmentRequest.CreatedAt,
                    LastUpdateAt = detail.ShipmentRequest.LastUpdateAt,
                    DeleteAt = detail.ShipmentRequest.DeleteAt,
                    CustomerId = detail.ShipmentRequest.CustomerId,
                    PickupLocationId = detail.ShipmentRequest.PickupLocationId,
                    DropoffLocationId = detail.ShipmentRequest.DropoffLocationId,
                    DeliveryDate = detail.ShipmentRequest.DeliveryDate,
                    Weight = detail.ShipmentRequest.Weight,
                    Volume = detail.ShipmentRequest.Volume,
                    Description = detail.ShipmentRequest.Description,
                    IsFragile = detail.ShipmentRequest.IsFragile,
                    HandlingNote = detail.ShipmentRequest.HandlingNote,
                    Status = detail.ShipmentRequest.Status,
                    ImageUrl = detail.ShipmentRequest.ImageUrl
                }
            };
        }

        public async Task<IEnumerable<GetShipmentDTO>> GetMyShipmentsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new AppException(400, "ID khong hop le");
            }

            var shipments = await _shipmentRepo.GetQueryable()
                .AsNoTracking()
                .Where(s => s.ShipmentOffer != null &&
                            (s.ShipmentOffer.DriverId == userId ||
                             s.ShipmentOffer.Request.CustomerId == userId))
                .Select(s => new GetShipmentDTO
                {
                    Id = s.Id,
                    OfferId = s.ShipmentOffer!.OfferId,
                    ShipmentOfferId = s.ShipmentOfferId,
                    DeliveryDate = s.ShipmentOffer.Request != null
                        ? s.ShipmentOffer.Request.DeliveryDate
                        : DateTime.MinValue,
                    ShipmentStatus = s.ShipmentStatus,
                    PaymentStatus = s.PaymentStatus
                })
                .ToListAsync();

            return shipments;
        }

        public async Task ReturnDelivery(Guid shipmentId)
        {
            var shipment = await _shipmentRepo.GetByIdAsync(shipmentId) ?? throw new AppException(404, "Không t?n t?i ??n hàng");
            shipment.ShipmentStatus = ShipmentStatus.Return;
            shipment.Price = shipment.Price * 2;

            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
            return;

        }

        public async Task Compelete(Guid shipmentId)
        {
            var shipment = await _shipmentRepo.GetByIdAsync(shipmentId) ?? throw new AppException(404, "Không t?n t?i ??n hàng");
            shipment.ShipmentStatus = ShipmentStatus.Delivered;
            shipment.DeliveredDate = DateTime.Now;

            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid userId, UpdateShipmentDTO request)
        {
            if (userId == Guid.Empty)
            {
                throw new AppException(400, "ID khong hop le");
            }

            var shipment = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .ThenInclude(so => so.Request)
                .FirstOrDefaultAsync(s => s.Id == request.Id)
                ?? throw new AppException(404, "Khong tim thay ban ghi");

            if (shipment.ShipmentOffer == null)
            {
                throw new AppException(404, "Khong tim thay thong tin don van chuyen");
            }

            var isOwner = shipment.ShipmentOffer.DriverId == userId ||
                          (shipment.ShipmentOffer.Request != null && shipment.ShipmentOffer.Request.CustomerId == userId);
            if (!isOwner)
            {
                throw new AppException(403, "Ban khong duoc phep sua ban ghi nay");
            }

            if (request.ShipmentStatus is not null)
            {
                shipment.ShipmentStatus = (ShipmentStatus)request.ShipmentStatus;
            }

            if (request.PaymentStatus is not null)
            {
                shipment.PaymentStatus = (PaymentStatus)request.PaymentStatus;
            }

            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
        }

        public async Task CancelAsync(Guid userId, Guid shipmentId)
        {
            if (userId == Guid.Empty || shipmentId == Guid.Empty)
            {
                throw new AppException(400, "ID không h?p l?");
            }

            var shipment = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .ThenInclude(so => so.Request)
                .FirstOrDefaultAsync(s => s.Id == shipmentId)
                ?? throw new AppException(404, "Không tìm th?y b?n ghi");

            if (shipment.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm th?y b?n ghi");
            }

            if (shipment.ShipmentOffer == null)
            {
                throw new AppException(404, "Không tìm th?y thông tin ??n v?n chuy?n");
            }

            var isOwner = shipment.ShipmentOffer.DriverId == userId ||
                          (shipment.ShipmentOffer.Request != null && shipment.ShipmentOffer.Request.CustomerId == userId);
            if (!isOwner)
            {
                throw new AppException(403, "B?n không ???c phép h?y b?n ghi này");
            }

            if (shipment.ShipmentStatus == ShipmentStatus.Delivered)
            {
                throw new AppException(400, "Không th? h?y ??n hàng ?ã giao");
            }

            if (shipment.ShipmentStatus == ShipmentStatus.Cancelled)
            {
                throw new AppException(400, "??n hàng ?ã b? h?y");
            }

            if (shipment.ShipmentStatus == ShipmentStatus.Return)
            {
                throw new AppException(400, "Không th? h?y ??n hàng ?ang hoàn tr?");
            }

            shipment.ShipmentStatus = ShipmentStatus.Cancelled;
            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
        }
    }
}
