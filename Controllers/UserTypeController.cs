using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.Models;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userTypes = await _context.UserTypes.ToListAsync();
            
            return Ok(userTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userType = await _context.UserTypes.FindAsync(id);
            if(userType == null) return NotFound("Role Not Found!!");
            return Ok(userType);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserType userType)
        {
            if (userType == null)
                return BadRequest("User Type data is required.");

            await _context.UserTypes.AddAsync(userType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = userType.UserTypeID }, userType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserType userType)
        {
            if(userType == null)
                return BadRequest("User Type data is required.");
            if (id != userType.UserTypeID)
                return BadRequest("User Type ID mismatch.");
            
            var existingUserType = await _context.UserTypes.FindAsync(id);

            if (existingUserType == null)
                return NotFound("User Type Not Found !!");
            
            existingUserType.UserTypeName = userType.UserTypeName;
            existingUserType.Description = userType.Description;

            await _context.SaveChangesAsync();

            return Ok(existingUserType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userType = await _context.UserTypes.FindAsync(id);

            if (userType == null)
                return NotFound("User Type Not Found !!");
            
            _context.UserTypes.Remove(userType);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
