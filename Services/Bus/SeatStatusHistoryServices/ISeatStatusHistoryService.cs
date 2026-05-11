using AppGroupCar.DTO.RequestDTO.Bus.SeatStatusHistoryRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse;

namespace AppGroupCar.Services.Bus.SeatStatusHistoryServices
{
    public interface ISeatStatusHistoryService
    {
        Task<List<SeatStatusHistoryResponseDto>> ChangeStatusAsync(Guid userId, ChangeSeatStatusRequest dto);
        Task<List<SeatStatusHistoryResponseDto>> GetBySeatAsync(Guid seatId);
        Task<List<SeatStatusHistoryResponseDto>> GetByShowtimeAsync(Guid showtimeId);
    }
}
