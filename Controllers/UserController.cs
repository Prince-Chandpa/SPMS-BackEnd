using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.User;
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
            // var users = await _context.Users
            //     .Include(u => u.UserType)
            //     .ToListAsync();
            //
            // var result = users.Select(u => new UserDto
            // {
            //     UserID = u.UserID,
            //     UserTypeID = u.UserTypeID,
            //     FullName = u.FullName,
            //     UserCode = u.UserCode,
            //     Email = u.Email,
            //     MobileNumber = u.MobileNumber,
            //     ProfilePicturePath = u.ProfilePicturePath,
            //     IsActive = u.IsActive
            // });

            var result = await _context.Users
                .Join(
                    _context.UserTypes,
                    user => user.UserTypeID,
                    userType => userType.UserTypeID,
                    (user, userType) => new
                    {
                        UserID = user.UserID,
                        FullName = user.FullName,
                        UserCode = user.UserCode,
                        Email = user.Email,
                        MobileNumber = user.MobileNumber,
                        IsActive = user.IsActive,
                        UserTypeId = userType.UserTypeID,
                        UserTypeName = userType.UserTypeName,
                    }
                ).ToListAsync();
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.UserID == id);
            
            if(user == null) return NotFound("User Not Found!!");
            
            var result = new UserDto
            {
                UserID = user.UserID,
                UserTypeID = user.UserTypeID,
                FullName = user.FullName,
                UserCode = user.UserCode,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                ProfilePicturePath = user.ProfilePicturePath,
                IsActive = user.IsActive
            };
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = new User
            {
                UserTypeID = dto.UserTypeID,
                FullName = dto.FullName,
                UserCode = dto.UserCode,
                Email = dto.Email,
                Password = dto.Password,
                MobileNumber = dto.MobileNumber,
                ProfilePicturePath = dto.ProfilePicturePath,
                IsActive = dto.IsActive
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                UserID = user.UserID,
                UserTypeID = user.UserTypeID,
                FullName = user.FullName,
                UserCode = user.UserCode,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                ProfilePicturePath = user.ProfilePicturePath,
                IsActive = user.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = result.UserID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
                return NotFound("User Not Found !!");

            existingUser.UserTypeID = dto.UserTypeID;
            existingUser.FullName = dto.FullName;
            existingUser.UserCode = dto.UserCode;
            existingUser.Email = dto.Email;
            existingUser.Password = dto.Password;
            existingUser.MobileNumber = dto.MobileNumber;
            existingUser.ProfilePicturePath = dto.ProfilePicturePath;
            existingUser.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                UserID = existingUser.UserID,
                UserTypeID = existingUser.UserTypeID,
                FullName = existingUser.FullName,
                UserCode = existingUser.UserCode,
                Email = existingUser.Email,
                MobileNumber = existingUser.MobileNumber,
                ProfilePicturePath = existingUser.ProfilePicturePath,
                IsActive = existingUser.IsActive
            };
            
            return Ok(result);
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
