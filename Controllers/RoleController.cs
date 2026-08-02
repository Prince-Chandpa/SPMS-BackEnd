using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.Role;
using spm_backend.Models;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _context.Roles.ToListAsync();

            var result = roles.Select(r => new RoleDto
            {
                RoleID = r.RoleID,
                RoleName = r.RoleName,
                Description = r.Description,
                IsActive = r.IsActive
            });
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            
            if(role == null) return NotFound("Role Not Found!!");
            
            var result = new RoleDto
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive
            };
            
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            
            var result = new RoleDto
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = result.RoleID }, result);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            var existingRole = await _context.Roles.FindAsync(id);

            if (existingRole == null)
                return NotFound("Role Not Found !!");

            existingRole.RoleName = dto.RoleName;
            existingRole.Description = dto.Description;
            existingRole.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            
            var result = new RoleDto
            {
                RoleID = existingRole.RoleID,
                RoleName = existingRole.RoleName,
                Description = existingRole.Description,
                IsActive = existingRole.IsActive
            };

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
                return NotFound("Role Not Found !!");
            
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
