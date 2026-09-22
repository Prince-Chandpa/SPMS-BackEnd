using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
using spm_backend.Data;
using spm_backend.DTOs.Task;
using TaskModel = spm_backend.Models.Task;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateTaskDto> _createValidator;
        private readonly IValidator<UpdateTaskDto> _updateValidator;
        
        public TaskController(AppDbContext context, IValidator<CreateTaskDto> createValidator, IValidator<UpdateTaskDto> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? taskId,
            [FromQuery] string? taskTitle,
            [FromQuery] int? taskPriorityId,
            [FromQuery] int? taskStatusId,
            [FromQuery] decimal? assignedScore,
            [FromQuery] DateTime? dueDate,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
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
                
                var userId = GetCurrentUserId();
                
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Errors = new List<string> { "The JWT does not contain a valid User ID claim." }
                    });
                }
                
                var roles = GetCurrentUserRoles();

                var query = _context.Tasks
                    .AsNoTracking()
                    .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .AsQueryable();
                
                if (taskId.HasValue)
                {
                    query = query.Where(t => t.TaskID == taskId.Value);
                }
                
                if (!string.IsNullOrWhiteSpace(taskTitle))
                {
                    query = query.Where(t => t.TaskTitle.Contains(taskTitle));
                }
                
                if (taskPriorityId.HasValue)
                {
                    query = query.Where(t => t.TaskPriorityID == taskPriorityId.Value);
                }
                
                if (taskStatusId.HasValue)
                {
                    query = query.Where(t => t.TaskStatusID == taskStatusId.Value);
                }
                
                if (assignedScore.HasValue)
                {
                    query = query.Where(t => t.AssignedScore == assignedScore.Value);
                }
                
                if (dueDate.HasValue)
                {
                    query = query.Where(t => t.TaskDueDate.Value.Date == dueDate.Value.Date);
                }
                
                if (fromDate.HasValue)
                {
                    query = query.Where(t => t.TaskDueDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(t => t.TaskDueDate <= toDate.Value);
                }
                
                if (roles.Contains("Admin"))
                { }
                else if (roles.Contains("Faculty"))
                {
                    query = query.Where(t =>
                        t.ProjectAllocation.FacultyID == userId.Value);
                }
                else if (roles.Contains("Student"))
                {
                    query = query.Where(t =>
                        t.ProjectAllocation.StudentID == userId.Value);
                }
                else
                {
                    return Forbid();
                }

                var totalCount = await query.CountAsync();
                
                var result = await query
                    .Select(t => new TaskDto
                    {
                        TaskID = t.TaskID,
                        ProjectAllocationID = t.ProjectAllocationID,
                        ProjectTitle = t.ProjectAllocation.ProjectMaster.ProjectTitle,
                        TaskStatusID = t.TaskStatusID,
                        TaskStatusName = t.TaskStatus.TaskStatusName,
                        TaskPriorityID = t.TaskPriorityID,
                        TaskPriorityName = t.TaskPriority.TaskPriorityName,
                        TaskTitle = t.TaskTitle,
                        TaskDescription = t.TaskDescription,
                        AssignedScore = t.AssignedScore,
                        EarnedScore = t.EarnedScore,
                        ProgressPercentage = t.ProgressPercentage,
                        TaskAssignedDate = t.TaskAssignedDate,
                        TaskStartDate = t.TaskStartDate,
                        TaskDueDate = t.TaskDueDate,
                        TaskCompletedDate = t.TaskCompletedDate,
                        NextFollowUpDate = t.NextFollowUpDate,
                        FacultyRemarks = t.FacultyRemarks,
                        StudentRemarks = t.StudentRemarks,
                        IsActive = t.IsActive
                    })
                    .OrderBy(t => t.TaskDueDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Retrieved Successfully !!",
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
                    Message = "Error occurred while retrieving Tasks !!",
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
                
                var task = await _context.Tasks
                    .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .FirstOrDefaultAsync(t => t.TaskID == id);
                
                if (task == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Not Found !!",
                        Errors = new List<string> { $"No task found with Id {id}" }
                    });
                }
                
                var roles = GetCurrentUserRoles();

                if (roles.Contains("Admin"))
                { }
                else if (roles.Contains("Faculty"))
                {
                    if (task.ProjectAllocation.FacultyID != userId.Value)
                    {
                        return Forbid();
                    }
                }
                else if (roles.Contains("Student"))
                {
                    if (task.ProjectAllocation.StudentID != userId.Value)
                    {
                        return Forbid();
                    }
                }
                else
                {
                    return Forbid();
                }
                
                var result = new TaskDto
                {
                    TaskID = task.TaskID,
                    ProjectAllocationID = task.ProjectAllocationID,
                    ProjectTitle = task.ProjectAllocation.ProjectMaster.ProjectTitle,
                    TaskStatusID = task.TaskStatusID,
                    TaskStatusName = task.TaskStatus.TaskStatusName,
                    TaskPriorityID = task.TaskPriorityID,
                    TaskPriorityName = task.TaskPriority.TaskPriorityName,
                    TaskTitle = task.TaskTitle,
                    TaskDescription = task.TaskDescription,
                    AssignedScore = task.AssignedScore,
                    EarnedScore = task.EarnedScore,
                    ProgressPercentage = task.ProgressPercentage,
                    TaskAssignedDate = task.TaskAssignedDate,
                    TaskStartDate = task.TaskStartDate,
                    TaskDueDate = task.TaskDueDate,
                    TaskCompletedDate = task.TaskCompletedDate,
                    NextFollowUpDate = task.NextFollowUpDate,
                    FacultyRemarks = task.FacultyRemarks,
                    StudentRemarks = task.StudentRemarks,
                    IsActive = task.IsActive
                };
                
                return Ok(new ApiResponse<TaskDto>
                {
                    Success = true,
                    Message = "Task Retrieved Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving Task !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize(Roles = "Admin,Faculty")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
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
                
                var userIdClaim =
                    User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid")
                    ?? User.FindFirstValue("userId");

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid user identity."
                    });
                }

                var roles = User
                    .FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                
                var projectAllocation = await _context.ProjectAllocations
                    .FirstOrDefaultAsync(pa =>
                        pa.ProjectAllocationID == dto.ProjectAllocationID);

                if (projectAllocation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project allocation not found."
                    });
                }
                
                if (roles.Contains("Faculty") &&
                    projectAllocation.FacultyID != userId)
                {
                    return Forbid();
                }
                
                var taskStatusExists = await _context.TaskStatuses
                    .AnyAsync(ts => ts.TaskStatusID == dto.TaskStatusID);

                if (!taskStatusExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid TaskStatusID."
                    });
                }
                
                var taskPriorityExists = await _context.TaskPriorities
                    .AnyAsync(tp => tp.TaskPriorityID == dto.TaskPriorityID);

                if (!taskPriorityExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid TaskPriorityID."
                    });
                }

                var task = new TaskModel
                {
                    ProjectAllocationID = dto.ProjectAllocationID,
                    TaskStatusID = dto.TaskStatusID,
                    TaskPriorityID = dto.TaskPriorityID,
                    TaskTitle = dto.TaskTitle,
                    TaskDescription = dto.TaskDescription,
                    AssignedScore = dto.AssignedScore,
                    EarnedScore = dto.EarnedScore,
                    ProgressPercentage = dto.ProgressPercentage,
                    TaskAssignedDate = dto.TaskAssignedDate,
                    TaskStartDate = dto.TaskStartDate,
                    TaskDueDate = dto.TaskDueDate,
                    TaskCompletedDate = dto.TaskCompletedDate,
                    NextFollowUpDate = dto.NextFollowUpDate,
                    FacultyRemarks = dto.FacultyRemarks,
                    StudentRemarks = dto.StudentRemarks,
                    IsActive = dto.IsActive
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                var createdTask = await _context.Tasks
                    .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .FirstAsync(t => t.TaskID == task.TaskID);

                if (createdTask == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task was created but could not be retrieved."
                    });
                }
                
                var result = new TaskDto
                {
                    TaskID = createdTask.TaskID,
                    ProjectAllocationID = createdTask.ProjectAllocationID,
                    ProjectTitle = createdTask.ProjectAllocation?.ProjectMaster?.ProjectTitle ?? string.Empty,
                    TaskStatusID = createdTask.TaskStatusID,
                    TaskStatusName = createdTask.TaskStatus?.TaskStatusName ?? string.Empty,
                    TaskPriorityID = createdTask.TaskPriorityID,
                    TaskPriorityName = createdTask.TaskPriority?.TaskPriorityName ?? string.Empty,
                    TaskTitle = createdTask.TaskTitle,
                    TaskDescription = createdTask.TaskDescription,
                    AssignedScore = createdTask.AssignedScore,
                    EarnedScore = createdTask.EarnedScore,
                    ProgressPercentage = createdTask.ProgressPercentage,
                    TaskAssignedDate = createdTask.TaskAssignedDate,
                    TaskStartDate = createdTask.TaskStartDate,
                    TaskDueDate = createdTask.TaskDueDate,
                    TaskCompletedDate = createdTask.TaskCompletedDate,
                    NextFollowUpDate = createdTask.NextFollowUpDate,
                    FacultyRemarks = createdTask.FacultyRemarks,
                    StudentRemarks = createdTask.StudentRemarks,
                    IsActive = createdTask.IsActive
                };

                return Ok(new ApiResponse<TaskDto>
                {
                    Success = true,
                    Message = "Task Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating Task !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]  UpdateTaskDto dto)
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
                
                var userIdClaim =
                    User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid")
                    ?? User.FindFirstValue("userId");

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid user identity."
                    });
                }

                var roles = User
                    .FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                var task = await _context.Tasks
                    .Include(t => t.ProjectAllocation)
                    .FirstOrDefaultAsync(t => t.TaskID == id);

                if (task == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Not Found !!",
                    });
                }
                
                if (task.ProjectAllocation == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task project allocation was not found !!"
                    });
                }
                
                if (roles.Contains("Admin"))
                {
                }
                else if (roles.Contains("Faculty"))
                {
                    if (task.ProjectAllocation.FacultyID != userId)
                    {
                        return Forbid();
                    }
                }
                else if (roles.Contains("Student"))
                {
                    if (task.ProjectAllocation.StudentID != userId)
                    {
                        return Forbid();
                    }
                }
                else
                {
                    return Forbid();
                }

                if (!await _context.ProjectAllocations.AnyAsync(pa =>
                        pa.ProjectAllocationID == dto.ProjectAllocationID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Project Allocation ID."
                    });
                }

                if (!await _context.TaskStatuses.AnyAsync(ts => ts.TaskStatusID == dto.TaskStatusID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Task Status ID."
                    });
                }

                if (!await _context.TaskPriorities.AnyAsync(tp => tp.TaskPriorityID == dto.TaskPriorityID))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Task Priority ID."
                    });
                }

                if (roles.Contains("Admin"))
                {
                    task.ProjectAllocationID = dto.ProjectAllocationID;
                    task.TaskStatusID = dto.TaskStatusID;
                    task.TaskPriorityID = dto.TaskPriorityID;
                    task.TaskTitle = dto.TaskTitle;
                    task.TaskDescription = dto.TaskDescription;
                    task.AssignedScore = dto.AssignedScore;
                    task.EarnedScore = dto.EarnedScore;
                    task.ProgressPercentage = dto.ProgressPercentage;
                    task.TaskAssignedDate = dto.TaskAssignedDate;
                    task.TaskStartDate = dto.TaskStartDate;
                    task.TaskDueDate = dto.TaskDueDate;
                    task.TaskCompletedDate = dto.TaskCompletedDate;
                    task.NextFollowUpDate = dto.NextFollowUpDate;
                    task.FacultyRemarks = dto.FacultyRemarks;
                    task.StudentRemarks = dto.StudentRemarks;
                    task.IsActive = dto.IsActive;
                }
                else if (roles.Contains("Faculty"))
                {
                    task.TaskStatusID = dto.TaskStatusID;
                    task.TaskPriorityID = dto.TaskPriorityID;
                    task.TaskTitle = dto.TaskTitle;
                    task.TaskDescription = dto.TaskDescription;
                    task.EarnedScore = dto.EarnedScore;
                    task.ProgressPercentage = dto.ProgressPercentage;
                    task.TaskStartDate = dto.TaskStartDate;
                    task.TaskDueDate = dto.TaskDueDate;
                    task.TaskCompletedDate = dto.TaskCompletedDate;
                    task.NextFollowUpDate = dto.NextFollowUpDate;
                    task.FacultyRemarks = dto.FacultyRemarks;
                    task.IsActive = dto.IsActive;
                }
                else if (roles.Contains("Student"))
                {
                    task.TaskStatusID = dto.TaskStatusID;
                    task.ProgressPercentage = dto.ProgressPercentage;
                    task.TaskStartDate = dto.TaskStartDate;
                    task.TaskCompletedDate = dto.TaskCompletedDate;
                    task.NextFollowUpDate = dto.NextFollowUpDate;
                    task.StudentRemarks = dto.StudentRemarks;
                }

                await _context.SaveChangesAsync();

                var updatedTask = await _context.Tasks
                    .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .FirstAsync(t => t.TaskID == task.TaskID);

                if (updatedTask == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task was updated but could not be retrieved."
                    });
                }
                
                var result = new TaskDto
                {
                    TaskID = updatedTask.TaskID,
                    ProjectAllocationID = updatedTask.ProjectAllocationID,
                    ProjectTitle = updatedTask.ProjectAllocation?.ProjectMaster?.ProjectTitle,
                    TaskStatusID = updatedTask.TaskStatusID,
                    TaskStatusName = updatedTask.TaskStatus?.TaskStatusName,
                    TaskPriorityID = updatedTask.TaskPriorityID,
                    TaskPriorityName = updatedTask.TaskPriority?.TaskPriorityName,
                    TaskTitle = updatedTask.TaskTitle,
                    TaskDescription = updatedTask.TaskDescription,
                    AssignedScore = updatedTask.AssignedScore,
                    EarnedScore = updatedTask.EarnedScore,
                    ProgressPercentage = updatedTask.ProgressPercentage,
                    TaskAssignedDate = updatedTask.TaskAssignedDate,
                    TaskStartDate = updatedTask.TaskStartDate,
                    TaskDueDate = updatedTask.TaskDueDate,
                    TaskCompletedDate = updatedTask.TaskCompletedDate,
                    NextFollowUpDate = updatedTask.NextFollowUpDate,
                    FacultyRemarks = updatedTask.FacultyRemarks,
                    StudentRemarks = updatedTask.StudentRemarks,
                    IsActive = updatedTask.IsActive
                };

                return Ok(new ApiResponse<TaskDto>
                {
                    Success = true,
                    Message = "Task Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Error occurred while updating Task !!",
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
                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Not Found !!"
                    });
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Deleted Successfully !!",
                    Data = task
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Error occurred while deleting Task !!",
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