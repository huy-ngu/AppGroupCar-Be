using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.ResponseDTO.Search;

namespace AppGroupCar.Services.Search
{
    public interface ITripSearchService
    {
        Task<PagedResult<TripSearchItemDTO>> SearchTripsAsync(TripSearchParameters parameters);
    }
}
