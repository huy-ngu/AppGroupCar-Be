using AppGroupCar.Entities.Bus;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.BookingRequest
{
    public class CreateBusBookingRequest
    {
        [Required]
        public Guid ShowtimeId { get; set; }

        [Required]
        public BusBookingStatus Status { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> SeatNumbers { get; set; } = new();
    }
}
