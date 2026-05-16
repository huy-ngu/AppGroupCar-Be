namespace AppGroupCar.DTO.ResponseDTO.Bus.CompanyDriverResponse
{
    public class CompanyDriverDetailResponseDto
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public Guid UserId { get; set; }

        public string UserFullName { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public string LicenseNumber { get; set; } = null!;

        public string LicenseClass { get; set; } = null!;

        public int DriverVerificationStatus { get; set; }

        public string? LicenseDocumentUrl { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public Guid? VerifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
