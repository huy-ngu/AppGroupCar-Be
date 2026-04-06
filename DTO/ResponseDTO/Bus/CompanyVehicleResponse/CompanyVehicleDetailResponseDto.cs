namespace AppGroupCar.DTO.ResponseDTO.Bus.CompanyVehicleResponse
{
    public class CompanyVehicleDetailResponseDto : CompanyVehicleResponseDto
    {
        public string? CompanyName { get; set; }
        public string? SeatLayoutName { get; set; }
        public string? SeatLayoutDescription { get; set; }
        public bool? SeatLayoutIsActive { get; set; }
    }
}
