using AppGroupCar.Entities.Market;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO
{
    public class UpdateRideRequestDTO
    {
        [DefaultValue("cc0cfa65-2bb4-4cf8-bff4-08de78310217")]
        public Guid Id { get; set; }
        [DefaultValue("2576628b-2325-45a4-33ad-08de76eb22d1")]
        public Guid? FromLocationId { get; set; }
        [DefaultValue("a4054762-067d-4627-35bc-08de76ebe4bb")]
        public Guid? ToLocationId { get; set; }
        public DateTime? DesiredDepartureTime { get; set; }
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Số ghế phải lớn hơn 0!")]
        public int? SeatsRequired { get; set; }

        public RideRequestStatus Status { get; set; }
    }
}
