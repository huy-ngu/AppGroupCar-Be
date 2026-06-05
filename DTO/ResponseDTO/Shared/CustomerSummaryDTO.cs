namespace AppGroupCar.DTO.ResponseDTO.Shared
{
    public class CustomerSummaryDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
