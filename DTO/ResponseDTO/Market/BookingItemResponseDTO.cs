namespace AppGroupCar.DTO.ResponseDTO
{
    public class BookingItemResponseDTO
    {
        public Guid ItemId { get; set; }

        public Guid BookingId { get; set; }

        public Guid PickupLocationId { get; set; }

        public Guid DropoffLocationId { get; set; }

        public decimal Price { get; set; }

    }
}
