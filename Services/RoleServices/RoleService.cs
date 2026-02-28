using AppGroupCar.Data;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.RoleServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task CreateRole(string RoleName)
        {
            var role = await _roleManager.CreateAsync(new ApplicationRole { Name = RoleName, NormalizedName = RoleName.ToUpper() });
            if (!role.Succeeded)
            {
                throw new AppException(400, role.Errors.ToString());
            }

            return;
        }

        public async Task DeleteRole(Guid Id)
        {
            var role = await _roleManager.FindByIdAsync(Id.ToString());
            if (role == null)
            {
                throw new AppException(404, "Not found role");
            }
            if (role.Name == "Admin")
            {
                throw new AppException(403, "Không được xóa Admin!");
            }
            var hasUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId.ToString() == role.Id.ToString());
            if (hasUsers)
            {
                throw new AppException(403, "Không được xóa Role này!");
            }
            var rs = await _roleManager.DeleteAsync(role);
            if (!rs.Succeeded)
            {
                throw new AppException(400, rs.Errors.ToString());
            }
            return;
        }

        public async Task<List<RoleResponseDTO>> GetAllRole()
        {
            List<RoleResponseDTO> roles = await _context.Roles.Select(r => new RoleResponseDTO
            { Id = r.Id, Name = r.Name, UserCount = _context.UserRoles.Count(ur => ur.RoleId.ToString() == r.Id.ToString()) }).ToListAsync();
            return roles;
        }


    }
}
