using AppGroupCar.Entities.Market;
using System.ComponentModel;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class VehicleDTO
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        [DefaultValue("")]
        public string PlateNumber { get; set; }
        [DefaultValue("")]
        public string Brand { get; set; }
        public int SeatCapacity { get; set; }
        public VehicleType VehicleType { get; set; }
        public VehicleStatus Status { get; set; }
        public string UrlImage { get; set; }
        public string RegistrationDocumentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
    }
}
