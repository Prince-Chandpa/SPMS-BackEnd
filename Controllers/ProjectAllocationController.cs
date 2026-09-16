using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
using spm_backend.Data;
using spm_backend.DTOs.ProjectAllocation;
using spm_backend.Models;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateProjectAllocationDto> _createValidator;
        private readonly IValidator<UpdateProjectAllocationDto> _updateValidator;
        
        public ProjectAllocationController(AppDbContext context, IValidator<CreateProjectAllocationDto> createValidator, IValidator<UpdateProjectAllocationDto> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = GetCurrentUserId();
        
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Errors = new List<string>
                        {
                            "The JWT does not contain a valid User ID claim."
                        }
                    });
                }
        
                var roles = GetCurrentUserRoles();
        
                var query = _context.ProjectAllocations
                    .AsNoTracking()
                    .Include(pa => pa.ProjectMaster)
                    .Include(pa => pa.UserStudent)
                    .Include(pa => pa.UserFaculty)
                    .AsQueryable();
        
                if (roles.Contains("Admin"))
                {
                }
                else if (roles.Contains("Faculty"))
                {
                    query = query.Where(pa => pa.FacultyID == userId.Value);
                }
                else if (roles.Contains("Student"))
                {
                    query = query.Where(pa => pa.StudentID == userId.Value);
                }
                else
                {
                    return Forbid();
                }
        
                var result = await query
                    .Select(pa => new ProjectAllocationDto
                    {
                        ProjectAllocationID = pa.ProjectAllocationID,
                        ProjectID = pa.ProjectID,
                        ProjectTitle = pa.ProjectMaster.ProjectTitle,
                        StudentID = pa.StudentID,
                        StudentName = pa.UserStudent.FullName,
                        FacultyID = pa.FacultyID,
                        FacultyName = pa.UserFaculty.FullName,
                        AssignedDate = pa.AssignedDate,
                        ProjectStartDate = pa.ProjectStartDate,
                        ProjectEndDate = pa.ProjectEndDate,
                        TotalTasksGiven = pa.TotalTasksGiven,
                        TotalCompletedTasks = pa.TotalCompletedTasks,
                        ProgressPercentage = pa.ProgressPercentage,
                        OverAllGrade = pa.OverAllGrade,
                        IsActive = pa.IsActive
                    })
                    .ToListAsync();
        
                return Ok(new ApiResponse<List<ProjectAllocationDto>>
                {
                    Success = true,
                    Message = "Project Allocations Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Project Allocations !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User ID not found in token"
                    });
                }

                var projectAllocation = await _context.ProjectAllocations
                    .Include(pa => pa.ProjectMaster)
                    .Include(pa => pa.UserStudent)
                    .Include(pa => pa.UserFaculty)
                    .FirstOrDefaultAsync(pa => pa.ProjectAllocationID == id);
                
                var roles = GetCurrentUserRoles();

                if (!roles.Contains("Admin"))
                {
                    if (roles.Contains("Faculty") &&
                        projectAllocation.FacultyID != userId.Value)
                    {
                        return Forbid();
                    }

                    if (roles.Contains("Student") &&
                        projectAllocation.StudentID != userId.Value)
                    {
                        return Forbid();
                    }
                }
                
                if (projectAllocation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found !!",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }
                
                var result = new ProjectAllocationDto
                {
                    ProjectAllocationID = projectAllocation.ProjectAllocationID,
                    ProjectID = projectAllocation.ProjectID,
                    ProjectTitle = projectAllocation.ProjectMaster.ProjectTitle,
                    StudentID = projectAllocation.StudentID,
                    StudentName = projectAllocation.UserStudent.FullName,
                    FacultyID = projectAllocation.FacultyID,
                    FacultyName = projectAllocation.UserFaculty.FullName,
                    AssignedDate = projectAllocation.AssignedDate,
                    ProjectStartDate = projectAllocation.ProjectStartDate,
                    ProjectEndDate = projectAllocation.ProjectEndDate,
                    TotalTasksGiven = projectAllocation.TotalTasksGiven,
                    TotalCompletedTasks = projectAllocation.TotalCompletedTasks,
                    ProgressPercentage = projectAllocation.ProgressPercentage,
                    OverAllGrade = projectAllocation.OverAllGrade,
                    IsActive = projectAllocation.IsActive
                };
                
                return Ok(new ApiResponse<ProjectAllocationDto>
                {
                    Success = true,
                    Message = "Project Allocation Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Project Allocation !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectAllocationDto dto)
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
                
                var projectExists = await _context.ProjectMasters
                    .AnyAsync(p =>
                        p.ProjectMasterID == dto.ProjectID &&
                        p.IsActive &&
                        !p.IsDeleted);

                if (!projectExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Project ID",
                        Errors = new List<string> { "Project does not exist or is inactive" }
                    });
                }
                
                var studentExists = await _context.UserRoles
                    .AnyAsync(ur =>
                        ur.UserID == dto.StudentID &&
                        ur.Role != null &&
                        ur.Role.RoleName == "Student" &&
                        ur.Role.IsActive &&
                        !ur.Role.IsDeleted &&
                        ur.User != null &&
                        ur.User.IsActive &&
                        !ur.User.IsDeleted);

                if (!studentExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Student ID",
                        Errors = new List<string> { "Selected user is not an active Student" }
                    });
                }
                
                var facultyExists = await _context.UserRoles
                    .AnyAsync(ur =>
                        ur.UserID == dto.FacultyID &&
                        ur.Role != null &&
                        ur.Role.RoleName == "Faculty" &&
                        ur.Role.IsActive &&
                        !ur.Role.IsDeleted &&
                        ur.User != null &&
                        ur.User.IsActive &&
                        !ur.User.IsDeleted);

                if (!facultyExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Faculty ID",
                        Errors = new List<string> { "Selected user is not an active Faculty" }
                    });
                }

                var projectAllocation = new ProjectAllocation
                {
                    ProjectID = dto.ProjectID,
                    StudentID = dto.StudentID,
                    FacultyID = dto.FacultyID,
                    AssignedDate = dto.AssignedDate,
                    ProjectStartDate = dto.ProjectStartDate,
                    ProjectEndDate = dto.ProjectEndDate,
                    TotalTasksGiven = dto.TotalTasksGiven,
                    TotalCompletedTasks = dto.TotalCompletedTasks,
                    ProgressPercentage = dto.ProgressPercentage,
                    OverAllGrade = dto.OverAllGrade,
                    IsActive = dto.IsActive
                };

                _context.ProjectAllocations.Add(projectAllocation);
                await _context.SaveChangesAsync();

                var created = await _context.ProjectAllocations
                    .Include(pa => pa.ProjectMaster)
                    .Include(pa => pa.UserStudent)
                    .Include(pa => pa.UserFaculty)
                    .FirstAsync(pa => pa.ProjectAllocationID == projectAllocation.ProjectAllocationID);

                var result = new ProjectAllocationDto
                {
                    ProjectAllocationID = created.ProjectAllocationID,
                    ProjectID = created.ProjectID,
                    ProjectTitle = created.ProjectMaster.ProjectTitle,
                    StudentID = created.StudentID,
                    StudentName = created.UserStudent.FullName,
                    FacultyID = created.FacultyID,
                    FacultyName = created.UserFaculty.FullName,
                    AssignedDate = created.AssignedDate,
                    ProjectStartDate = created.ProjectStartDate,
                    ProjectEndDate = created.ProjectEndDate,
                    TotalTasksGiven = created.TotalTasksGiven,
                    TotalCompletedTasks = created.TotalCompletedTasks,
                    ProgressPercentage = created.ProgressPercentage,
                    OverAllGrade = created.OverAllGrade,
                    IsActive = created.IsActive
                };
                
                return Ok(new ApiResponse<ProjectAllocationDto>
                {
                    Success = true,
                    Message = "Project Allocation Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating Project Allocation !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody] UpdateProjectAllocationDto dto)
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
                
                var existingProjectAllocation = await _context.ProjectAllocations.FindAsync(id);

                if (existingProjectAllocation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found !!",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                var projectExists = await _context.ProjectMasters
                    .AnyAsync(p =>
                        p.ProjectMasterID == dto.ProjectID &&
                        p.IsActive &&
                        !p.IsDeleted);

                if (!projectExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Project ID"
                    });
                }
                
                var studentExists = await _context.UserRoles
                    .AnyAsync(ur =>
                        ur.UserID == dto.StudentID &&
                        ur.Role != null &&
                        ur.Role.RoleName == "Student" &&
                        ur.Role.IsActive &&
                        !ur.Role.IsDeleted &&
                        ur.User != null &&
                        ur.User.IsActive &&
                        !ur.User.IsDeleted);

                if (!studentExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Student ID"
                    });
                }

                var facultyExists = await _context.UserRoles
                    .AnyAsync(ur =>
                        ur.UserID == dto.FacultyID &&
                        ur.Role != null &&
                        ur.Role.RoleName == "Faculty" &&
                        ur.Role.IsActive &&
                        !ur.Role.IsDeleted &&
                        ur.User != null &&
                        ur.User.IsActive &&
                        !ur.User.IsDeleted);

                if (!facultyExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Faculty ID"
                    });
                }

                existingProjectAllocation.ProjectID = dto.ProjectID;
                existingProjectAllocation.StudentID = dto.StudentID;
                existingProjectAllocation.FacultyID = dto.FacultyID;
                existingProjectAllocation.AssignedDate = dto.AssignedDate;
                existingProjectAllocation.ProjectStartDate = dto.ProjectStartDate;
                existingProjectAllocation.ProjectEndDate = dto.ProjectEndDate;
                existingProjectAllocation.TotalTasksGiven = dto.TotalTasksGiven;
                existingProjectAllocation.TotalCompletedTasks = dto.TotalCompletedTasks;
                existingProjectAllocation.ProgressPercentage = dto.ProgressPercentage;
                existingProjectAllocation.OverAllGrade = dto.OverAllGrade;
                existingProjectAllocation.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                var updated = await _context.ProjectAllocations
                    .Include(pa => pa.ProjectMaster)
                    .Include(pa => pa.UserStudent)
                    .Include(pa => pa.UserFaculty)
                    .FirstAsync(pa => pa.ProjectAllocationID == existingProjectAllocation.ProjectAllocationID);

                var result = new ProjectAllocationDto
                {
                    ProjectAllocationID = updated.ProjectAllocationID,
                    ProjectID = updated.ProjectID,
                    ProjectTitle = updated.ProjectMaster.ProjectTitle,
                    StudentID = updated.StudentID,
                    StudentName = updated.UserStudent.FullName,
                    FacultyID = updated.FacultyID,
                    FacultyName = updated.UserFaculty.FullName,
                    AssignedDate = updated.AssignedDate,
                    ProjectStartDate = updated.ProjectStartDate,
                    ProjectEndDate = updated.ProjectEndDate,
                    TotalTasksGiven = updated.TotalTasksGiven,
                    TotalCompletedTasks = updated.TotalCompletedTasks,
                    ProgressPercentage = updated.ProgressPercentage,
                    OverAllGrade = updated.OverAllGrade,
                    IsActive = updated.IsActive
                };
                
                return Ok(new ApiResponse<ProjectAllocationDto>
                {
                    Success = true,
                    Message = "Project Allocation Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ProjectAllocationDto>
                {
                    Success = false,
                    Message = "Error occurred while updating Project Allocation !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var projectAllocation = await _context.ProjectAllocations.FindAsync(id);
                
                if(projectAllocation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found !!"
                    });
                }
                
                _context.ProjectAllocations.Remove(projectAllocation);
                await _context.SaveChangesAsync();
                
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Allocation Deleted Successfully !!",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ProjectAllocationDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting Project Allocation !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetAllDropDown()
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User ID not found in token"
                    });
                }
                
                var query = _context.ProjectAllocations
                    .AsNoTracking()
                    .Include(x => x.ProjectMaster)
                    .Include(x => x.UserStudent)
                    .Include(x => x.UserFaculty)
                    .AsQueryable();
                
                var roles = GetCurrentUserRoles();
                
                if (roles.Contains("Admin"))
                {
                }
                else if (roles.Contains("Faculty"))
                {
                    query = query.Where(x => x.FacultyID == userId.Value);
                }
                else if (roles.Contains("Student"))
                {
                    query = query.Where(x => x.StudentID == userId.Value);
                }
                else
                {
                    return Forbid();
                }
                
                var result = await query
                    .OrderBy(x => x.ProjectMaster.ProjectTitle)
                    .Select(x => new ProjectAllocationDto
                    {
                        ProjectAllocationID = x.ProjectAllocationID,
                        ProjectTitle = x.ProjectMaster.ProjectTitle,
                        StudentName = x.UserStudent.FullName,
                        FacultyName = x.UserFaculty.FullName
                    }).ToListAsync();
                    
                return Ok(new ApiResponse<List<ProjectAllocationDto>>
                {
                    Success = true,
                    Message = "Project Allocations Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Project Allocation Dropdown !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("nameid")
                ?? User.FindFirstValue("userId");

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return userId;
        }

        private List<string> GetCurrentUserRoles()
        {
            return User
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }
}