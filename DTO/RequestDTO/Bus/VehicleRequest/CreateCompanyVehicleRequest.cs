using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Market;
using Microsoft.AspNetCore.Http;

namespace AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest
{
    public class CreateCompanyVehicleRequest
    {
        [Required]
        public Guid SeatLayoutId { get; set; }

        [Required, MaxLength(20)]
        public string PlateNumber { get; set; } = null!;

        [Required]
        public IFormFile UrlImage { get; set; } = null!;

        [Required]
        public IFormFile RegistrationDocumentImage { get; set; } = null!;

        [Required]
        public int SeatCapacity { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public VehicleStatus Status { get; set; }
    }
}
