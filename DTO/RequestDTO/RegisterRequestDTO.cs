namespace AppGroupCar.DTO.RequestDTO
{
    public class RegisterRequestDTO
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public string PhoneNumberConfirmed { get; set; }
        public string Password { get; set; }
        public IFormFile? Avatar { get; set; }

    }
}
