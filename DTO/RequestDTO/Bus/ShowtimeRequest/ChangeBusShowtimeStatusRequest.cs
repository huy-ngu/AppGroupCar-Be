using AppGroupCar.Entities.Bus;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest
{
    public class ChangeBusShowtimeStatusRequest
    {
        [Required]
        public BusShowtimeStatus Status { get; set; }
    }
}
