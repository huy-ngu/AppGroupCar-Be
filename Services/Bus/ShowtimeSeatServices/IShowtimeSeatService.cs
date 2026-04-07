using AppGroupCar.DTO.ResponseDTO.Bus.SeatResponse;

namespace AppGroupCar.Services.Bus.ShowtimeSeatServices
{
    public interface IShowtimeSeatService
    {
        Task<List<ShowtimeSeatResponseDto>> GetSeatMapAsync(Guid showtimeId);
    }
}
