using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Vehicle
{
    public class CreateVehicleDTO
    {
        [Required(ErrorMessage = "Plate number is required")]
        [MaxLength(20, ErrorMessage = "Plate number must not exceed 20 characters")]
        public string PlateNumber { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [MaxLength(50, ErrorMessage = "Brand must not exceed 50 characters")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Seat capacity is required")]
        [Range(1, 50, ErrorMessage = "Seat capacity must be between 1 and 50")]
        public int SeatCapacity { get; set; }

        [Required(ErrorMessage = "Vehicle type is required")]
        public VehicleType VehicleType { get; set; }

        [Required(ErrorMessage = "Vehicle image is required")]
        public IFormFile UrlImage { get; set; }

        [Required(ErrorMessage = "Registration document is required")]
        public IFormFile RegistrationDocumentUrl { get; set; }
    }
}
