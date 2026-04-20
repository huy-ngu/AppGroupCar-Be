using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.MarketDriverServices
{
    public interface IMarketDriverService
    {
        Task<PagedResult<Driver>> GetMarketDriversAsync(DriverParameters request);
        Task<Driver> CreatAsync(Guid id, CreateDriverRequestDTO request);
        Task<Driver> UpdateAsync(Guid id, UpdateDriverRequestDTO request);
        Task<Driver> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task ApprovalDriverAsync(Guid id);
    }
}
