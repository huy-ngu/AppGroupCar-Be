
using AppGroupCar.Data;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.RoleServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RoleService(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task CreateRole(string RoleName)
        {
            var role = await _roleManager.CreateAsync(new IdentityRole { Name = RoleName, NormalizedName = RoleName.ToUpper() });
            if (!role.Succeeded)
            {
                throw new AppException(400, role.Errors.ToString());
            }

            return;
        }

        public async Task DeleteRole(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                throw new AppException(404, "Not found role");
            }
            if (role.Name == "Admin")
            {
                throw new AppException(403, "Không được xóa Admin!");
            }
            var userRole = await _context.UserRoles.Select(ur => ur.RoleId == role.Id).FirstOrDefaultAsync();
            if (userRole != null)
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
            { Id = r.Id, Name = r.Name, UserCount = _context.UserRoles.Count(ur => ur.RoleId == r.Id) }).ToListAsync();
            return roles;
        }


    }
}
