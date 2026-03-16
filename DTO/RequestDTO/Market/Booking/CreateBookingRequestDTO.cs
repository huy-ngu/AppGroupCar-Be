using AppGroupCar.Entities.Market;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO.Market.Booking
{
    public class CreateBookingRequestDTO
    {
        [DefaultValue("3de6663e-4016-4278-4b4a-08de7f263702")]
        public Guid OfferId { get; set; }
        [DefaultValue("189fce95-911d-4a59-3705-08de76e0be83")]
        public Guid CustomerId { get; set; }

    }
}
