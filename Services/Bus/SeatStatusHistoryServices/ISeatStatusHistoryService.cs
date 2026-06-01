using AppGroupCar.DTO.RequestDTO.Bus.SeatStatusHistoryRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse;
using AppGroupCar.DTO.ResponseDTO.Bus.CheckInResponse;

namespace AppGroupCar.Services.Bus.SeatStatusHistoryServices
{
    public interface ISeatStatusHistoryService
    {
        Task<List<SeatStatusHistoryResponseDto>> ChangeStatusAsync(Guid companyId, Guid userId, ChangeSeatStatusRequest dto);
        Task<List<SeatStatusHistoryResponseDto>> GetBySeatAsync(Guid seatId);
        Task<List<SeatStatusHistoryResponseDto>> GetByShowtimeAsync(Guid showtimeId);
        Task<SeatCheckInInfoResponseDto> GetCheckInInfoAsync(Guid companyId, Guid showtimeId, string seatNumber);
    }
}
