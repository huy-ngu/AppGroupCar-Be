using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AppGroupCar.DTO.RequestDTO.Bus.SeatLayoutRequest
{
    public class CreateVehicleSeatLayoutRequest
    {
        [Required]
        public string SeatLayoutName { get; set; }

        [Required]
        public int SeatCapacity { get; set; }

        [Required]
        public JsonElement LayoutJson { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
