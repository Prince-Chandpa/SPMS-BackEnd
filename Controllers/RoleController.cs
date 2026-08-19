using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
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
        private readonly IValidator<CreateRoleDto> _createValidator;
        private readonly IValidator<UpdateRoleDto> _updateValiator;
        
        public RoleController(AppDbContext context, IValidator<CreateRoleDto> createValidator, IValidator<UpdateRoleDto> updateValiator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValiator = updateValiator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.Roles.Select(r => new RoleDto
            {
                RoleID = r.RoleID,
                RoleName = r.RoleName,
                Description = r.Description,
                IsActive = r.IsActive
            }).ToListAsync();
            
            return Ok(new ApiResponse<List<RoleDto>>
            {
                Success = true,
                Message = "Role Retrieved Successfully !!",
                Data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            
            if(role == null)
            {
                return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Role Not Found !!",
                        Errors = new List<string> { $"No role found with Id {id}" }
                    }
                );
            }
            
            var result = new RoleDto
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive
            };
            
            return Ok(new ApiResponse<RoleDto>
            {
                Success = true,
                Message = "Role Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
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

                return Ok(new ApiResponse<RoleDto>
                {
                    Success = true,
                    Message = "Role Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating role !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRoleDto dto)
        {
            try
            {
                var validator = await _updateValiator.ValidateAsync(dto);
                
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
                
                var existingRole = await _context.Roles.FindAsync(id);

                if (existingRole == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Role Not Found !!",
                        Errors = new List<string> { $"No Role found with Id {id}" }
                    });
                }

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

                return Ok(new ApiResponse<RoleDto>
                {
                    Success = false,
                    Message = "Role Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<RoleDto>
                {
                    Success = false,
                    Message = "Error occurred while updating Role !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                if (role == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Role Not Found !!"
                    });
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Role Deleted Successfully !!",
                    Data = role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<RoleDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting Role !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}