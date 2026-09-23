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
    [Authorize(Roles="Admin")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IValidator<CreateUserDto> _createValidator;
        private readonly IValidator<UpdateUserDto> _updateValidator;
        private readonly IFileService _fileService;
        
        public UserController(AppDbContext context, TokenService tokenService, IValidator<CreateUserDto> createValidator, IValidator<UpdateUserDto> updateValidator, IFileService fileService)
        {
            _context = context;
            _tokenService = tokenService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _fileService = fileService;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _context.Users
                    .SingleOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password && u.IsActive && !u.IsDeleted);
                
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Email or password",
                        Errors = new List<string> {"Invalid Password or Email"}
                    });
                }

                var roles = await _context.UserRoles
                    .Where(ur => ur.UserID == user.UserID && ur.Role != null && ur.Role.IsActive && !ur.Role.IsDeleted)
                    .Select(ur => ur.Role!.RoleName)
                    .ToListAsync();

                if (roles.Count == 0)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User does not have any active role assigned",
                        Errors = new List<string> { "No active role assigned to this user" }
                    });
                }
                    
                var token = _tokenService.GenerateToken(user, roles);
                
                return Ok(new ApiResponse<object>
                { 
                    Success = true,
                    Message = "Login Successfully !!",
                    Data = new
                    {
                        Token = token
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while logging in",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? fullName,
            [FromQuery] string? email,
            [FromQuery] int? userTypeId,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Page number or size invalid",
                        Errors = new List<string> { "Page number or size must be greater than 0" }
                    });
                }

                var query = _context.Users
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    query = query.Where(u => u.FullName.Contains(fullName));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(u => u.FullName.Contains(email));
                }
                
                if (userTypeId.HasValue)
                {
                    query = query.Where(u => u.UserTypeID == userTypeId.Value);
                }
                
                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }
                
                var totalCount = await query.CountAsync();
                
                var result = await query
                    .Select(u => new UserDto
                    {
                        UserID = u.UserID,
                        UserTypeID = u.UserTypeID,
                        FullName = u.FullName,
                        UserCode = u.UserCode,
                        Email = u.Email,
                        MobileNumber = u.MobileNumber,
                        ProfilePicturePath = u.ProfilePicturePath,
                        IsActive = u.IsActive
                    })
                    .OrderBy(u => u.FullName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

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
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Retrieved Successfully !!",
                    Data = new
                    {
                        result,
                        pageNumber,
                        pageSize,
                        totalCount,
                        totalPage = (int)Math.Ceiling(totalCount / (double) pageSize)
                    }
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            try
            {
                var validator = await _createValidator.ValidateAsync(dto);
                string? uploadedPath = null;
                
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

                if (dto.ProfilePicture != null)
                {
                    uploadedPath = await _fileService.UploadFileAsync(dto.ProfilePicture, "Users");
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
                    ProfilePicturePath = uploadedPath,
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateUserDto dto)   
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
                
                if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
                {
                    var oldProfilePicturePath = existingUser.ProfilePicturePath;
                    
                    var newProfilePicturePath = await _fileService.UploadFileAsync(dto.ProfilePicture, "Users");
                    
                    existingUser.ProfilePicturePath = newProfilePicturePath;
                    
                    _fileService.DeleteFile(oldProfilePicturePath);
                }
                
                existingUser.UserTypeID = dto.UserTypeID;
                existingUser.FullName = dto.FullName;
                existingUser.UserCode = dto.UserCode;
                existingUser.Email = dto.Email;
                existingUser.Password = dto.Password;
                existingUser.MobileNumber = dto.MobileNumber;
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] bool deleteFileOnly = false)
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

                if (deleteFileOnly)
                {
                    if (string.IsNullOrEmpty(user.ProfilePicturePath))
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Profile Picture Not Found !!",
                            Errors = new List<string> { "No document exists for this user." }
                       });
                    }
                    
                    _fileService.DeleteFile(user.ProfilePicturePath);
                    user.ProfilePicturePath = null;
                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Document deleted successfully.",
                    });
                }
            
                _fileService.DeleteFile(user.ProfilePicturePath);
                _context.Users.Remove(user);            
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Deleted Successfully !!",
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
