using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.MarketDriver
{
    public class UpdateDriverRequestDTO
    {
        [MaxLength(20)]
        public string? IdentityNumber { get; set; } = null!;

        [MaxLength(20)]
        public string? LicenseNumber { get; set; } = null!;

        [MaxLength(10)]
        public string? LicenseClass { get; set; } = null!;

        public DriverVerificationStatus? VerificationStatus { get; set; }

        public IFormFile? LicenseDocumenImg { get; set; }
    }
}
