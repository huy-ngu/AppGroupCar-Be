using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.RouteRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;

namespace AppGroupCar.Services.Bus.BusRoute
{
    public interface IBusRouteService
    {
        Task<BusRouteResponse> CreateAsync(
        Guid companyId,
        CreateBusRouteRequest dto);

        Task<List<BusRouteResponse>> GetByCompanyAsync(
            Guid companyId);

        Task<BusRouteResponse?> GetByIdAsync(
            Guid id,
            Guid companyId);

        Task<BusRouteResponse> UpdateAsync(
            Guid id,
            Guid companyId,
            UpdateBusRouteRequest dto);

        Task DeleteAsync(
            Guid id,
            Guid companyId);

        Task<bool> ExistsForCompanyAsync(
            Guid routeId,
            Guid companyId);

        Task<PagedResult<BusRouteResponse>> GetAsync(Guid companyId, BusRouteParameters param);
    }
}
