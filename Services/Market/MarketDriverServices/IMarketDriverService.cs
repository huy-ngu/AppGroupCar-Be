using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.MarketDriverServices
{
    public interface IMarketDriverService
    {
        Task<Driver> CreatAsync(Guid id, string fullName, CreateDriverRequestDTO request);
        Task<Driver> UpdateAsync(Guid id, UpdateDriverRequestDTO request);
        Task<Driver> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task<PagedResult<Driver>> FilterAsync(MarketDriverParameters param);

    }
}
