using System;

namespace AppGroupCar.DTO.RequestDTO.Bus.UCRequest
{
    public class LeaveCompanyRequestDto
    {
        public Guid? CompanyUserId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
