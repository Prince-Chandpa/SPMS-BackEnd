using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
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
            
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if(role == null) return NotFound("Role Not Found!!");
            return Ok(role);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (role == null)
                return BadRequest("Role data is required.");

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = role.RoleID }, role);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Role role)
        {
            if(role == null)
                return BadRequest("Role data is required.");
            if (id != role.RoleID)
                return BadRequest("Role ID mismatch.");
            
            var existingRole = await _context.Roles.FindAsync(id);

            if (existingRole == null)
                return NotFound("Role Not Found !!");

            existingRole.RoleName = role.RoleName;
            existingRole.Description = role.Description;

            await _context.SaveChangesAsync();

            return Ok(existingRole);
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
