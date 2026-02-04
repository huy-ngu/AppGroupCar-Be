using AppGroupCar.DTO.ResponseDTO;
using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Services.RoleServices
{
    public interface IRoleService
    {
        Task<List<RoleResponseDTO>> GetAllRole();
        Task CreateRole(string RoleName);
        Task DeleteRole(string Id);
    }
}
