using AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;

namespace AppGroupCar.Services.Bus.ShowtimeServices
{
    public interface IBusShowtimeService
    {
        Task<BusShowtimeResponse> CreateAsync(
            Guid companyId,
            Guid userId,
            CreateBusShowtimeRequest dto);

        Task<List<BusShowtimeResponse>> GetByCompanyAsync(
            Guid companyId);

        Task<List<BusShowtimeResponse>> FilterAsync(
            Guid companyId,
            DateOnly departureDate,
            string? plateNumber,
            TimeOnly? departureTime);

        Task<BusShowtimeResponse> UpdateAsync(
            Guid id,
            Guid companyId,
            UpdateBusShowtimeRequest dto);

        Task DeleteAsync(
            Guid id,
            Guid companyId,
            Guid userId);

        Task<BusShowtimeResponse> ChangeStatusAsync(
            Guid id,
            Guid companyId,
            Guid userId,
            ChangeBusShowtimeStatusRequest dto);
    }
}
