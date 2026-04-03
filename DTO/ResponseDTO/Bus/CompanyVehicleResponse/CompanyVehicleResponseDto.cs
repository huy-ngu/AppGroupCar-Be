namespace AppGroupCar.DTO.ResponseDTO.Bus.CompanyVehicleResponse
{
    public class CompanyVehicleResponseDto
    {
        public Guid CompanyVehicleId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid SeatLayoutId { get; set; }
        public string PlateNumber { get; set; } = null!;
        public string UrlImage { get; set; } = null!;
        public string RegistrationDocumentUrl { get; set; } = null!;
        public int SeatCapacity { get; set; }
        public int VehicleType { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
