using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
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

            if (request.ImageUrl != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(request.ImageUrl);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            var shipmentRequest = _mapper.Map<ShipmentRequest>(request);
            shipmentRequest.CustomerId = userId;
            shipmentRequest.Status = ShipmentRequestStatus.Open;
            shipmentRequest.ImageUrl = imgUrl;

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

        public async Task<PagedResult<ShipmentRequest>> FilterAsync(ShipmentRequestParameters param)
        {
            var data = await _repo.FindAndFilter(
                predicate: r =>
                    //  Lọc điểm đi (nếu null/rỗng thì bỏ qua điều kiện này)
                    (string.IsNullOrEmpty(param.pickUpLocation) || r.PickupLocation.Name.Contains(param.pickUpLocation)) &&

                    // Lọc điểm đến (đã sửa lại thành ToLocation)
                    (string.IsNullOrEmpty(param.dropOffLocation) || r.DropoffLocation.Name.Contains(param.dropOffLocation)) &&

                r.Status == ShipmentRequestStatus.Open && r.DeleteAt == null,


                orderBy: q => q.OrderByDescending(r => r.PickupLocation.Name),
                pageNumber: param.PageNumber,
                pageSize: param.PageSize
            );

            return data;
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
            if (exitShipmentRequest == null || exitShipmentRequest?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }

            string? imgUrl = null;

            if (request.imgFile != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(request.imgFile);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            _mapper.Map(request, exitShipmentRequest);

            if (request.DropoffLocationId != null)
            {
                exitShipmentRequest.DropoffLocationId = (Guid)request.DropoffLocationId;
            }
            if (request.PickupLocationId != null && request.PickupLocationId is Guid)
            {
                exitShipmentRequest.PickupLocationId = (Guid)request.PickupLocationId;
            }

            exitShipmentRequest.ImageUrl = imgUrl;
            _repo.Update(exitShipmentRequest);
            await _repo.SaveChangesAsync();
            return;
        }
    }
}
