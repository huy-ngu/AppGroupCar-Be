using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AppGroupCar.DTO.ResponseDTO.Bus.SeatLayoutResponse
{
    public class VehicleSeatLayoutResponseDto
    {
        public Guid Id { get; set; }
        public string SeatLayoutName { get; set; }
        public int SeatCapacity { get; set; }
        public JsonElement LayoutJson { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
