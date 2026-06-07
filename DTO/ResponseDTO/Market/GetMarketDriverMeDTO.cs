using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class GetMarketDriverMeDTO : GetMarketDriverDetailDTO
    {
        public string IdentityNumber { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public DriverVerificationStatus VerificationStatus { get; set; }
    }
}
