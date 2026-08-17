using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Repositories;
using spm_backend.Common;
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
            var result = await _context.UserTypes.Select(ut => new UserTypeDto
            {
                UserTypeID = ut.UserTypeID,
                UserTypeName = ut.UserTypeName,
                Description = ut.Description,
                IsActive = ut.IsActive
            }).ToListAsync();
            
            return Ok(new ApiResponse<List<UserTypeDto>>
            {
                Success = true,
                Message = "User Type Retrieved Successfully !!",
                Data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userType = await _context.UserTypes.FindAsync(id);
            
            if(userType == null)
            {
                return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Type Not Found !!",
                        Errors = new List<string> { $"No user type found with Id {id}" }
                    }
                );
            }

            var result = new UserTypeDto
            {
                UserTypeID = userType.UserTypeID,
                UserTypeName = userType.UserTypeName,
                Description = userType.Description,
                IsActive = userType.IsActive
            };
            
            return Ok(new ApiResponse<UserTypeDto>
            {
                Success = true,
                Message = "User Type Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTypeDto dto)
        {
            try
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

                return Ok(new ApiResponse<UserTypeDto>
                {
                    Success = true,
                    Message = "User Type Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating user type !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserTypeDto dto)
        {
            try
            {
                var existingUserType = await _context.UserTypes.FindAsync(id);

                if (existingUserType == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Type Not Found !!",
                        Errors = new List<string> { $"No User Type found with Id {id}" }
                    });
                }

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

                return Ok(new ApiResponse<UserTypeDto>
                {
                    Success = false,
                    Message = "User Type Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserTypeDto>
                {
                    Success = false,
                    Message = "Error occurred while updating User Type !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userType = await _context.UserTypes.FindAsync(id);

                if (userType == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Type Not Found !!"
                    });
                }

                _context.UserTypes.Remove(userType);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Type Deleted Successfully !!",
                    Data = userType
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserTypeDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting User Type !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
