using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;

namespace AppGroupCar.Services.Auth.UserServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(string id);
        Task CreateAsync(CreateUserDTO dto);
        Task UpdateAsync(string id, UpdateUserDTO dto);
        Task DeleteAsync(string id);
        Task ChangePassword(Guid id, string oldPassword, string newPassword);
    }
}
