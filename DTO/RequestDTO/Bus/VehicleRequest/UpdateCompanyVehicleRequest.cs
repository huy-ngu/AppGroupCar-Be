using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest
{
    public class UpdateCompanyVehicleRequest
    {
        public Guid? SeatLayoutId { get; set; }

        public IFormFile? UrlImage { get; set; }

        public IFormFile? RegistrationDocumentImage { get; set; }

        public int? SeatCapacity { get; set; }

        public int? VehicleType { get; set; }

        public int? Status { get; set; }
    }
}
