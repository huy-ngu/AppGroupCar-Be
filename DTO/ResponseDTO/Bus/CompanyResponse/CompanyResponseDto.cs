namespace AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse
{
    public class CompanyResponseDto
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; } = null!;

        public string CompanyCode { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string Phone { get; set; } = null!;

        public string? Email { get; set; }

        public int CompanyType { get; set; }

        public CompanyStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
