using AppGroupCar.Entities.Bus;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.SeatStatusHistoryRequest
{
    public class ChangeSeatStatusRequest
    {
        [Required]
        public Guid ShowtimeId { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> SeatNumbers { get; set; } = new();

        [Required]
        public SeatStatus NewStatus { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }
    }
}
