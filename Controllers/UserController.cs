using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
using spm_backend.Data;
using spm_backend.DTOs.User;
using spm_backend.Models;
using spm_backend.Services;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IValidator<CreateUserDto> _createValidator;
        private readonly IValidator<UpdateUserDto> _updateValidator;
        
        public UserController(AppDbContext context, TokenService tokenService, IValidator<CreateUserDto> createValidator, IValidator<UpdateUserDto> updateValidator)
        {
            _context = context;
            _tokenService = tokenService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _context.Users
                    .SingleOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Email or password",
                        Errors = new List<string> {"Invalid Password or Email"}
                    });
                }
        
                var token = _tokenService.GenerateToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _context.Users
                    .Include(u => u.UserType).Select(u => new UserDto
                    {
                        UserID = u.UserID,
                        UserTypeID = u.UserTypeID,
                        FullName = u.FullName,
                        UserCode = u.UserCode,
                        Email = u.Email,
                        MobileNumber = u.MobileNumber,
                        ProfilePicturePath = u.ProfilePicturePath,
                        IsActive = u.IsActive
                    }).ToListAsync();

                // var result = await _context.Users
                //     .Join(
                //         _context.UserTypes,
                //         user => user.UserTypeID,
                //         userType => userType.UserTypeID,
                //         (user, userType) => new
                //         {
                //             UserID = user.UserID,
                //             FullName = user.FullName,
                //             UserCode = user.UserCode,
                //             Email = user.Email,
                //             MobileNumber = user.MobileNumber,
                //             IsActive = user.IsActive,
                //             UserTypeID = userType.UserTypeID,
                //             UserTypeName = userType.UserTypeName,
                //         }
                //     ).ToListAsync();

                // return Ok(result);
                return Ok(new ApiResponse<List<UserDto>>
                {
                    Success = true,
                    Message = "User Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Users !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.UserID == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "User Not Found !!",
                            Errors = new List<string> { $"No user found with Id {id}" }
                        }
                    );
                }

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

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = "User Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving User !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                var validator = await _createValidator.ValidateAsync(dto);

                if (!validator.IsValid)
                {
                    return BadRequest(new ApiResponse<Object>
                    {
                        Success = false,
                        Message = "Validation Failed",
                        Errors = validator.Errors
                            .GroupBy(x => x.PropertyName)
                            .Select(x => $"{x.Key}: {string.Join(", ", x.Select(e => e.ErrorMessage))}")
                            .ToList()
                    });
                }
                
                if (!await _context.UserTypes.AnyAsync(pa => pa.UserTypeID == dto.UserTypeID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid User Type ID."
                    });
                }
                
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

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = "User Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating User !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var validator = await _updateValidator.ValidateAsync(dto);

                if (!validator.IsValid)
                {
                    return BadRequest(new ApiResponse<Object>
                    {
                        Success = false,
                        Message = "Validation Failed",
                        Errors = validator.Errors
                            .GroupBy(x => x.PropertyName)
                            .Select(x => $"{x.Key}: {string.Join(", ", x.Select(e => e.ErrorMessage))}")
                            .ToList()
                    });
                }
                
                var existingUser = await _context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Not Found !!",
                        Errors = new List<string> { $"No user found with Id {id}" }
                    });
                }

                if (!await _context.UserTypes.AnyAsync(pa => pa.UserTypeID == dto.UserTypeID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid User Type ID."
                    });
                }
                
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
            
                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = "User Updated Successfully !!",
                    Data = result
                });   
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while updating User !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Not Found !!"
                    });
                }
            
                _context.Users.Remove(user);            
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Deleted Successfully !!",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting User !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetAllDropDown()
        {
            try
            {
                var result = await _context.Users
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.FullName)
                    .Select(x => new UserDto
                    {
                        UserID = x.UserID,
                        FullName = x.FullName,
                    }).ToListAsync();

                return Ok(new ApiResponse<List<UserDto>>
                {
                    Success = true,
                    Message = "Users Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving User Dropdown !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpGet("dropdown/students")]
        public async Task<IActionResult> GetStudentDropDown()
        {
            try
            {
                var result = await _context.Users
                    .AsNoTracking()
                    .Where(x => x.IsActive && !x.IsDeleted && x.UserType.UserTypeName == "Student")
                    .OrderBy(x => x.FullName)
                    .Select(x => new UserDto
                    {
                        UserID = x.UserID,
                        FullName = x.FullName,
                    }).ToListAsync();

                return Ok(new ApiResponse<List<UserDto>>
                {
                    Success = true,
                    Message = "Students Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Role Dropdown for Students !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpGet("dropdown/faculty")]
        public async Task<IActionResult> GetFacultyDropDown()
        {
            try
            {
                var result = await _context.Users
                    .AsNoTracking()
                    .Where(x => x.IsActive && !x.IsDeleted && x.UserType.UserTypeName == "Faculty")
                    .OrderBy(x => x.FullName)
                    .Select(x => new UserDto
                    {
                        UserID = x.UserID,
                        FullName = x.FullName,
                    }).ToListAsync();

                return Ok(new ApiResponse<List<UserDto>>
                {
                    Success = true,
                    Message = "Faculty Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Role Dropdown for Faculty !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
