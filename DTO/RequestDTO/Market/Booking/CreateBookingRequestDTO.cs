using AppGroupCar.DTO.RequestDTO.Market.BookingItem;
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
        [DefaultValue("b0a99078-12f2-46c3-4458-08de832ba639")]
        public Guid CustomerId { get; set; }
    }

}
