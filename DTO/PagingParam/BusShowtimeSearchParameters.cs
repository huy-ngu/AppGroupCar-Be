using AppGroupCar.DTO.Filters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.PagingParam
{
    public class BusShowtimeSearchParameters : PagingParameters
    {
        [Required]
        public Guid PickupLocationId { get; set; }

        [Required]
        public Guid DropoffLocationId { get; set; }

        [Required]
        [DefaultValue("2026-06-02")]
        public DateOnly StartDate { get; set; }

        [Required]
        [DefaultValue("2026-06-10")]
        public DateOnly EndDate { get; set; }

        [Required]
        [DefaultValue("08:00:00")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [DefaultValue("22:00:00")]
        public TimeOnly EndTime { get; set; }

        [Required]
        [DefaultValue(0)]
        public decimal StartPrice { get; set; }

        [Required]
        [DefaultValue(10000000)]
        public decimal EndPrice { get; set; }
    }
}
