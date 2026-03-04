namespace AppGroupCar.DTO.RequestDTO.User
{
    public class CreateUserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public IList<string> Roles { get; set; }
        public IFormFile? AvatarUrl { get; set; }
    }
}
