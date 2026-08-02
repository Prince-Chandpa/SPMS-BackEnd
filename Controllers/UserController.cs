using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.Models;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.UserType)
                .ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.UserID == id);
            if(user == null) return NotFound("User Not Found!!");
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]User user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetById), new { id = user.UserID }, user);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if(user == null)
                return BadRequest("User data is required.");
            if (id != user.UserID)
                return BadRequest("User ID mismatch.");
            
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
                return NotFound("User Not Found !!");

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.MobileNumber = user.MobileNumber;
            existingUser.UserTypeID = user.UserTypeID;
            existingUser.UserCode = user.UserCode;
            existingUser.ProfilePicturePath = user.ProfilePicturePath;
            existingUser.IsActive = user.IsActive;
            existingUser.IsDeleted = user.IsDeleted;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User Not Found !!");
            
            _context.Users.Remove(user);            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
