using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Services.Market.ShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market.ShipmentControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController(IShipmentService shipmentService) : ControllerBase
    {
        [HttpPatch]
        public async Task<IActionResult> CreateFromShipmentOffer(UpdateShipmentDTO request)
        {
            await shipmentService.UpdateAsync(request);
            return Ok(new ApiResponse<GetShipmentDTO> { Code = 200 });

        }
    }
}
