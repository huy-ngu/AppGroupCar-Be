using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail;
using AppGroupCar.DTO.ResponseDTO.Market;

namespace AppGroupCar.Services.Market.SharedRideOfferDetailServices
{
    public interface ISharedRideOfferDetailService
    {
        /// <summary>
        /// L?y danh sách t?t c? SharedRideOfferDetail (phân trang)
        /// </summary>
        Task<PagedResult<GetSharedRideOfferDetailDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// L?y chi ti?t SharedRideOfferDetail theo ID
        /// </summary>
        Task<GetSharedRideOfferDetailDTO> GetByIdAsync(Guid id);

        /// <summary>
        /// L?y chi ti?t theo OfferId
        /// </summary>
        Task<GetSharedRideOfferDetailDTO> GetByOfferIdAsync(Guid offerId);

        /// <summary>
        /// T?o SharedRideOfferDetail m?i
        /// </summary>
        Task<GetSharedRideOfferDetailDTO> CreateAsync(CreateSharedRideOfferDetailDTO request);

        /// <summary>
        /// C?p nh?t SharedRideOfferDetail
        /// </summary>
        Task<GetSharedRideOfferDetailDTO> UpdateAsync(Guid id, UpdateSharedRideOfferDetailDTO request);

        /// <summary>
        /// Xóa SharedRideOfferDetail
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
