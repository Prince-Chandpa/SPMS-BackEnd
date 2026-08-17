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
    public class ProjectAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectAllocationController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.ProjectAllocations
                .Include(pa => pa.ProjectMaster)
                .Include(pa => pa.UserStudent)
                .Include(pa => pa.UserFaculty)
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
                }).ToListAsync();
            
            return Ok(new ApiResponse<List<ProjectAllocationDto>>
            {
                Success = true,
                Message = "Project Allocations Retrieved Successfully !!",
                Data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var projectAllocation = await _context.ProjectAllocations
                .Include(pa => pa.ProjectMaster)
                .Include(pa => pa.UserStudent)
                .Include(pa => pa.UserFaculty)
                .FirstOrDefaultAsync(pa => pa.ProjectAllocationID == id);
            
            if (projectAllocation == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Project Allocation Not Fount !!",
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectAllocationDto dto)
        {
            try
            {
                if (!await _context.ProjectMasters.AnyAsync(p => p.ProjectMasterID == dto.ProjectID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Project ID",
                    });
                }

                if (!await _context.Users.AnyAsync(u => u.UserID == dto.StudentID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Student ID",
                    });
                }

                if (!await _context.Users.AnyAsync(u => u.UserID == dto.FacultyID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Faculty ID",
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
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody] UpdateProjectAllocationDto dto)
        {
            try
            {
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

                if (!await _context.ProjectMasters.AnyAsync(p => p.ProjectMasterID == dto.ProjectID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Project ID",
                    });
                }

                if (!await _context.Users.AnyAsync(u => u.UserID == dto.StudentID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Student ID",
                    });
                }

                if (!await _context.Users.AnyAsync(u => u.UserID == dto.FacultyID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Faculty ID",
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
                    Message = "Error occurred while updating project allocation !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

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
                        Message = "Project Allocation Not Fount !!"
                    });
                }
                
                _context.ProjectAllocations.Remove(projectAllocation);
                await _context.SaveChangesAsync();
                
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Allocation Deleted Successfully !!",
                    Data = projectAllocation
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ProjectAllocationDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting project allocation !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}