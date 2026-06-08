using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class GetMarketDriverDetailDTO
    {
        public Guid Id { get; set; }
        public string LicenseClass { get; set; } = null!;
        public string IdentityNumber { get; set; }
        public string LicenseNumber { get; set; }
        public decimal DriverRatingAverage { get; set; }
        public int DriverRatingCount { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = null!;
        public DriverVerificationStatus status { get; set; }
    }
}
