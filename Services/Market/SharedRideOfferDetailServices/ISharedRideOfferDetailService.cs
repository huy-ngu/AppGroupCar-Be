using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;

namespace AppGroupCar.Services.Market.SharedRideOfferDetailServices
{
    public interface ISharedRideOfferDetailService
    {
        Task<PagedResult<GetSharedRideOfferDetailDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

        Task<GetSharedRideOfferDetailDTO> GetByIdAsync(Guid id);

        Task<GetSharedRideOfferDetailDTO> GetByOfferIdAsync(Guid offerId);

        Task<GetSharedRideOfferDetailDTO> CreateAsync(CreateSharedRideOfferDetailDTO request);

        Task<GetSharedRideOfferDetailDTO> UpdateAsync(Guid id, UpdateSharedRideOfferDetailDTO request);

        Task DeleteAsync(Guid id);
    }
}
