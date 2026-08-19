using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
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
        private readonly IValidator<CreateUserRoleDto> _createValidator;
        private readonly IValidator<UpdateUserRoleDto> _updateValidator;
        
        public UserRoleController(AppDbContext context, IValidator<CreateUserRoleDto> createValidator, IValidator<UpdateUserRoleDto> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Select(ur => new UserRoleDto
                {
                    RolePermissionID = ur.RolePermissionID,
                    RoleID = ur.RoleID,
                    RoleName = ur.Role.RoleName ?? string.Empty,
                    UserID = ur.UserID,
                    UserName = ur.User.FullName ?? string.Empty
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<UserRoleDto>>
            {
                Success = true,
                Message = "User Role Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.RolePermissionID == id);

            if (userRole == null)
            {
                return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Role Not Found !!",
                        Errors = new List<string> { $"No user role found with Id {id}" }
                    }
                );
            }
            
            var result = new UserRoleDto
            {
                RolePermissionID = userRole.RolePermissionID,
                RoleID = userRole.RoleID,
                RoleName = userRole.Role?.RoleName ?? string.Empty,
                UserID = userRole.UserID,
                UserName = userRole.User?.FullName ?? string.Empty
            };

            return Ok(new ApiResponse<UserRoleDto>
            {
                Success = true,
                Message = "User Role Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRoleDto dto)
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
                
                var userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID);
                
                if (!userExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid User ID."
                    });
                }
                
                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
                
                if (!roleExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Role ID."
                    });
                }
            
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
            
                return Ok(new ApiResponse<UserRoleDto>
                {
                    Success = true,
                    Message = "User Role Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating user role !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]  UpdateUserRoleDto dto)
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
                
                var existingUserRole = await _context.UserRoles.FindAsync(id);

                if (existingUserRole == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Role Not Found !!",
                        Errors = new List<string> { $"No User Role found with Id {id}" }
                    });
                }

                var userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID);
                if (!userExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid User ID."
                    });
                }

                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
                if (!roleExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Role ID."
                    });
                }

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

                return Ok(new ApiResponse<UserRoleDto>
                {
                    Success = false,
                    Message = "User Role Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserRoleDto>
                {
                    Success = false,
                    Message = "Error occurred while updating User Role !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userRole = await _context.UserRoles.FindAsync(id);

                if (userRole == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Role Not Found !!"
                    });
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Role Deleted Successfully !!",
                    Data = userRole
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserRoleDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting User Role !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
