using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.DTO.ResponseDTO.Shared;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Services.Market.ShipmentRequestServices
{
    public class ShipmentRequestServices : IShipmentRequestServices
    {
        private readonly IBaseRepository<ShipmentRequest> _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public ShipmentRequestServices(IBaseRepository<ShipmentRequest> repo, IMapper mapper, IPhotoService photoService)
        {
            _repo = repo;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<ShipmentRequest> CreateAsync(Guid userId, CreateShipmentRequestDTO request)
        {
            string? imgUrl = null;

            if (request.ImageUrl != null && request.ImageUrl.Length > 0)
            {
                var uploadResult = await _photoService.UploadPhotoAsync(
                    request.ImageUrl,
                    "shipment-request-img"
                );

                if (uploadResult == null)
                    throw new AppException(400, "Image upload failed");

                if (uploadResult.Error != null)
                {
                    throw new AppException(
                        400,
                        "Cloudinary upload failed: " + uploadResult.Error.Message
                    );
                }

                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            var shipmentRequest = _mapper.Map<ShipmentRequest>(request);

            shipmentRequest.CustomerId = userId;
            shipmentRequest.Status = ShipmentRequestStatus.Open;
            shipmentRequest.ImageUrl = imgUrl;
            shipmentRequest.DropoffLocationId = request.DropoffLocationId;
            shipmentRequest.PickupLocationId = request.PickupLocationId;

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

        public async Task<PagedResult<ShipmentRequestFilterResponseDTO>> FilterAsync(ShipmentRequestParameters param)
        {
            var query = _repo.GetQueryable()
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.PickupLocation)
                .Include(r => r.DropoffLocation)
                .Where(r =>
                    (!param.PickupLocationId.HasValue || r.PickupLocationId == param.PickupLocationId.Value) &&
                    (!param.DropoffLocationId.HasValue || r.DropoffLocationId == param.DropoffLocationId.Value) &&
                    (!param.FromTime.HasValue || r.DeliveryDate >= param.FromTime.Value) &&
                    (!param.ToTime.HasValue || r.DeliveryDate <= param.ToTime.Value) &&
                    r.Status == ShipmentRequestStatus.Open &&
                    r.DeleteAt == null);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.DeliveryDate)
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .Select(r => new ShipmentRequestFilterResponseDTO
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    LastUpdateAt = r.LastUpdateAt,
                    DeleteAt = r.DeleteAt,
                    Customer = new CustomerSummaryDTO
                    {
                        Id = r.Customer.Id,
                        FullName = r.Customer.FullName,
                        PhoneNumber = r.Customer.PhoneNumber,
                        AvatarUrl = r.Customer.AvatarUrl
                    },
                    PickupLocation = new LocationSummaryDTO
                    {
                        Id = r.PickupLocation.Id,
                        Name = r.PickupLocation.Name
                    },
                    DropoffLocation = new LocationSummaryDTO
                    {
                        Id = r.DropoffLocation.Id,
                        Name = r.DropoffLocation.Name
                    },
                    DeliveryDate = r.DeliveryDate,
                    Weight = r.Weight,
                    Volume = r.Volume,
                    Description = r.Description,
                    IsFragile = r.IsFragile,
                    HandlingNote = r.HandlingNote,
                    Status = r.Status,
                    ImageUrl = r.ImageUrl
                })
                .ToListAsync();

            return new PagedResult<ShipmentRequestFilterResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize
            };
        }

        public async Task<ShipmentRequest> GetByIdAsync(Guid id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null || data?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            return data;
        }

        public async Task UpdateAsync(UpdateShipmentRequestDTO request)
        {
            var exitShipmentRequest = await _repo.GetByIdAsync(request.Id);

            if (exitShipmentRequest == null || exitShipmentRequest.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }

            _mapper.Map(request, exitShipmentRequest);

            if (request.imgFile != null && request.imgFile.Length > 0)
            {
                var uploadResult = await _photoService.UploadPhotoAsync(
                    request.imgFile,
                    "shipment-request-img"
                );

                if (uploadResult == null)
                {
                    throw new AppException(400, "Image upload failed");
                }

                if (uploadResult.Error != null)
                {
                    throw new AppException(
                        400,
                        "Cloudinary upload failed: " + uploadResult.Error.Message
                    );
                }

                exitShipmentRequest.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            if (request.DropoffLocationId != null)
            {
                exitShipmentRequest.DropoffLocationId = request.DropoffLocationId.Value;
            }

            if (request.PickupLocationId != null)
            {
                exitShipmentRequest.PickupLocationId = request.PickupLocationId.Value;
            }

            _repo.Update(exitShipmentRequest);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShipmentRequestFilterResponseDTO>> GetMyShipmentRequestsAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                throw new AppException(400, "ID khÃ´ng há»£p lá»‡");
            }

            return await _repo.GetQueryable()
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.PickupLocation)
                .Include(r => r.DropoffLocation)
                .Where(s => s.CustomerId == customerId && s.DeleteAt == null)
                .OrderByDescending(r => r.DeliveryDate)
                .Select(r => new ShipmentRequestFilterResponseDTO
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    LastUpdateAt = r.LastUpdateAt,
                    DeleteAt = r.DeleteAt,
                    Customer = new CustomerSummaryDTO
                    {
                        Id = r.Customer.Id,
                        FullName = r.Customer.FullName,
                        PhoneNumber = r.Customer.PhoneNumber,
                        AvatarUrl = r.Customer.AvatarUrl
                    },
                    PickupLocation = new LocationSummaryDTO
                    {
                        Id = r.PickupLocation.Id,
                        Name = r.PickupLocation.Name
                    },
                    DropoffLocation = new LocationSummaryDTO
                    {
                        Id = r.DropoffLocation.Id,
                        Name = r.DropoffLocation.Name
                    },
                    DeliveryDate = r.DeliveryDate,
                    Weight = r.Weight,
                    Volume = r.Volume,
                    Description = r.Description,
                    IsFragile = r.IsFragile,
                    HandlingNote = r.HandlingNote,
                    Status = r.Status,
                    ImageUrl = r.ImageUrl
                })
                .ToListAsync();
        }
    }
}
