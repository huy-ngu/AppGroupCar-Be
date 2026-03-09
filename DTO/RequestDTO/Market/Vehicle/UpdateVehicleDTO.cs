using AppGroupCar.Entities.Market;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Vehicle
{
    public class UpdateVehicleDTO
    {
        [MaxLength(20, ErrorMessage = "Plate number must not exceed 20 characters")]
        [DefaultValue("")]
        public string? PlateNumber { get; set; }

        [MaxLength(50, ErrorMessage = "Brand must not exceed 50 characters")]
        [DefaultValue("")]
        public string? Brand { get; set; }

        [Range(1, 50, ErrorMessage = "Seat capacity must be between 1 and 50")]
        public int? SeatCapacity { get; set; }

        public VehicleType? VehicleType { get; set; }

        public VehicleStatus? Status { get; set; }

        public IFormFile? UrlImage { get; set; }

        public IFormFile? RegistrationDocumentUrl { get; set; }
    }
}
