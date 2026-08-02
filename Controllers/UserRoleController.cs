using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.UserRole;
using spm_backend.Models;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserRoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();

            var result = userRoles.Select(ur => new UserRoleDto
            {
                RolePermissionID = ur.RolePermissionID,
                RoleID = ur.RoleID,
                RoleName = ur.Role?.RoleName ?? string.Empty,
                UserID = ur.UserID,
                UserName = ur.User?.FullName ?? string.Empty
            });

            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.RolePermissionID == id);

            if (userRole == null)
                return NotFound("User Role Not Found!!");
            
            var result = new UserRoleDto
            {
                RolePermissionID = userRole.RolePermissionID,
                RoleID = userRole.RoleID,
                RoleName = userRole.Role?.RoleName ?? string.Empty,
                UserID = userRole.UserID,
                UserName = userRole.User?.FullName ?? string.Empty
            };

            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRoleDto dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID);
            if (!userExists)
                return BadRequest("Invalid User ID.");

            var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
            if (!roleExists)
                return BadRequest("Invalid Role ID.");
            
            var userRole = new UserRole
            {
                RoleID = dto.RoleID,
                UserID = dto.UserID,
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            
            var createdUserRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .FirstAsync(ur => ur.RolePermissionID == userRole.RolePermissionID);

            var result = new UserRoleDto
            {
                RolePermissionID = createdUserRole.RolePermissionID,
                RoleID = createdUserRole.RoleID,
                RoleName = createdUserRole.Role?.RoleName ?? string.Empty,
                UserID = createdUserRole.UserID,
                UserName = createdUserRole.User?.FullName ?? string.Empty,
            };
            
            return CreatedAtAction(nameof(GetById), new { id = result.RolePermissionID }, result);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserRoleDto dto)
        {
            var existingUserRole = await _context.UserRoles.FindAsync(id);

            if (existingUserRole == null)
                return NotFound("User Role Not Found !!");
            
            var userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID);
            if (!userExists)
                return BadRequest("Invalid User ID.");

            var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
            if (!roleExists)
                return BadRequest("Invalid Role ID.");
            
            existingUserRole.RoleID = dto.RoleID;
            existingUserRole.UserID = dto.UserID;

            await _context.SaveChangesAsync();
            
            var updatedUserRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .FirstAsync(ur => ur.RolePermissionID == existingUserRole.RolePermissionID);
            
            var result = new UserRoleDto
            {
                RolePermissionID = updatedUserRole.RolePermissionID,
                RoleID = updatedUserRole.RoleID,
                RoleName = updatedUserRole.Role?.RoleName ?? string.Empty,
                UserID = updatedUserRole.UserID,
                UserName = updatedUserRole.User?.FullName ?? string.Empty
            };

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
                return NotFound("User Role Not Found !!");
            
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
