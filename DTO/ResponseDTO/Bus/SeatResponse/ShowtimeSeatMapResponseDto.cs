using System.Text.Json;

namespace AppGroupCar.DTO.ResponseDTO.Bus.SeatResponse
{
    public class ShowtimeSeatMapResponseDto
    {
        public Guid ShowtimeId { get; set; }
        public SeatLayoutSnapshotDto SeatLayout { get; set; } = new();
        public List<ShowtimeSeatResponseDto> Seats { get; set; } = new();
    }

    public class SeatLayoutSnapshotDto
    {
        public Guid Id { get; set; }
        public string SeatLayoutName { get; set; } = string.Empty;
        public int SeatCapacity { get; set; }
        public JsonElement LayoutJson { get; set; }
        public string? Description { get; set; }
    }
}
