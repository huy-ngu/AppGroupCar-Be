using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;

namespace AppGroupCar.Services.Auth.UserServices
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllAsync(UserParameters param);
        Task<UserDto> GetByIdAsync(string id);
        Task<UserDto> CreateAsync(CreateUserDTO dto);
        Task UpdateAsync(string id, UpdateUserDTO dto);
        Task DeleteAsync(string id);
        Task RestoreAsync(string id);
        Task ChangePassword(Guid id, string oldPassword, string newPassword);
    }
}
