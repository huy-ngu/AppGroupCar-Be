using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO.ResponseDTO;

namespace AppGroupCar.Services.UserServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(string id);
        Task CreateAsync(CreateUserDTO dto);
        Task UpdateAsync(string id, UpdateUserDTO dto);
        Task DeleteAsync(string id);
    }
}
