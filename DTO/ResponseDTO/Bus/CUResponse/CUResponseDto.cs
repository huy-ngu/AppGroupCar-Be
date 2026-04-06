namespace AppGroupCar.DTO.ResponseDTO.Bus.CUResponse
{
    public class CUResponseDto
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public Guid UserId { get; set; }

        public bool IsActive { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
