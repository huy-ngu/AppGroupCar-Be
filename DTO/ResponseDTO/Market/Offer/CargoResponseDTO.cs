using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.ResponseDTO.Market.Offer
{
    public class CargoResponseDTO
    {
        public decimal MaxWeight { get; set; }
        public decimal MaxVolume { get; set; }
        public bool AcceptFragile { get; set; }
    }
}
