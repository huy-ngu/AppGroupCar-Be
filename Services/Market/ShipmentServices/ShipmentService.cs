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
                throw new AppException(400, "ID không hợp lệ");
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
                throw new AppException(404, "Không tìm thấy đơn hàng");
            }

            var isOwner = detail.ShipmentOffer.DriverId == userId ||
                          detail.ShipmentRequest.CustomerId == userId;
            if (!isOwner)
            {
                throw new AppException(403, "Bạn không được phép xem đơn hàng này");
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
                throw new AppException(400, "ID không hợp lệ");
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
            var shipment = await _shipmentRepo.GetByIdAsync(shipmentId) ?? throw new AppException(404, "Kh�ng t?n t?i ??n h�ng");
            shipment.ShipmentStatus = ShipmentStatus.Return;
            shipment.Price = shipment.Price * 2;

            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
            return;

        }

        public async Task Compelete(Guid shipmentId)
        {
            var shipment = await _shipmentRepo.GetByIdAsync(shipmentId) ?? throw new AppException(404, "Kh�ng t?n t?i ??n h�ng");
            shipment.ShipmentStatus = ShipmentStatus.Delivered;
            shipment.DeliveredDate = DateTime.Now;

            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid userId, UpdateShipmentDTO request)
        {
            if (userId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var shipment = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .ThenInclude(so => so.Request)
                .FirstOrDefaultAsync(s => s.Id == request.Id)
                ?? throw new AppException(404, "Không tìm thấy bản ghi");

            if (shipment.ShipmentOffer == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin đơn vận chuyển");
            }

            var isOwner = shipment.ShipmentOffer.DriverId == userId ||
                          (shipment.ShipmentOffer.Request != null && shipment.ShipmentOffer.Request.CustomerId == userId);
            if (!isOwner)
            {
                throw new AppException(403, "Bạn không được phép sửa đơn hàng này");
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
                throw new AppException(400, "ID không hợp lệ");
            }

            var shipment = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .ThenInclude(so => so.Request)
                .FirstOrDefaultAsync(s => s.Id == shipmentId)
                ?? throw new AppException(404, "Không tìm thấy bản ghi");

            if (shipment.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }

            if (shipment.ShipmentOffer == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin đơn vận chuyển");
            }

            var isOwner = shipment.ShipmentOffer.DriverId == userId ||
                          (shipment.ShipmentOffer.Request != null && shipment.ShipmentOffer.Request.CustomerId == userId);
            if (!isOwner)
            {
                throw new AppException(403, "Bạn không được phép hủy đơn hàng này");
            }

            if (shipment.ShipmentStatus == ShipmentStatus.Delivered)
            {
                throw new AppException(400, "Không thể hủy đơn hàng đã giao");
            }

            if (shipment.ShipmentStatus == ShipmentStatus.Cancelled)
            {
                throw new AppException(400, "Đơn hàng đã bị hủy");
            }

            if (shipment.ShipmentStatus == ShipmentStatus.Return)
            {
                throw new AppException(400, "Không thể hủy đơn hàng đang hoàn trả");
            }

            shipment.ShipmentStatus = ShipmentStatus.Cancelled;
            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
        }

        public async Task DriverUpdateStatusAsync(Guid userId, UpdateShipmentStatusDTO request)
        {
            if (userId == Guid.Empty || request.ShipmentId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var shipment = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId)
                ?? throw new AppException(404, "Không tìm thấy bản ghi");

            if (shipment.ShipmentOffer == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin đơn vận chuyển");
            }

            if (shipment.ShipmentOffer.DriverId != userId)
            {
                throw new AppException(403, "Bạn không phải là tài xế của chuyến đi này");
            }

            shipment.ShipmentStatus = request.ShipmentStatus;

            if (request.ShipmentStatus == ShipmentStatus.Delivered)
            {
                shipment.DeliveredDate = DateTime.Now;
            }

            _shipmentRepo.Update(shipment);
            await _shipmentRepo.SaveChangesAsync();
        }
    }
}
