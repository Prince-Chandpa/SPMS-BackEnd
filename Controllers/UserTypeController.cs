using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.UserType;
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

            var result = userTypes.Select(ut => new UserTypeDto
            {
                UserTypeID = ut.UserTypeID,
                UserTypeName = ut.UserTypeName,
                Description = ut.Description,
                IsActive = ut.IsActive
            });
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userType = await _context.UserTypes.FindAsync(id);
            
            if(userType == null) return NotFound("User Type Not Found!!");

            var result = new UserTypeDto
            {
                UserTypeID = userType.UserTypeID,
                UserTypeName = userType.UserTypeName,
                Description = userType.Description,
                IsActive = userType.IsActive
            };
            
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTypeDto dto)
        {
            var userType = new UserType
            {
                UserTypeName = dto.UserTypeName,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.UserTypes.Add(userType);
            await _context.SaveChangesAsync();

            var result = new UserTypeDto
            {
                UserTypeID = userType.UserTypeID,
                UserTypeName = userType.UserTypeName,
                Description = userType.Description,
                IsActive = userType.IsActive
            };
            
            return CreatedAtAction(nameof(GetById), new { id = result.UserTypeID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserTypeDto dto)
        {
            var existingUserType = await _context.UserTypes.FindAsync(id);

            if (existingUserType == null)
                return NotFound("User Type Not Found !!");
            
            existingUserType.UserTypeName = dto.UserTypeName;
            existingUserType.Description = dto.Description;
            existingUserType.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            
            var result = new UserTypeDto
            {
                UserTypeID = existingUserType.UserTypeID,
                UserTypeName = existingUserType.UserTypeName,
                Description = existingUserType.Description,
                IsActive = existingUserType.IsActive
            };

            return Ok(result);
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
