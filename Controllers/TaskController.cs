using FluentValidation;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.ProjectMaster)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority).Select(t => new TaskDto
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
                }).ToListAsync();
            
            return Ok(new ApiResponse<List<TaskDto>>
            {
                Success = true,
                Message = "Task Retrieved Successfully !!",
                Data = result
            });
        }
       
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
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
                
                if (!await _context.ProjectAllocations.AnyAsync(pa => pa.ProjectAllocationID == dto.ProjectAllocationID))
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
                
                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Not Found !!",
                        Errors = new List<string> { $"No task found with Id {id}" }
                    });
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

                await _context.SaveChangesAsync();

                var updatedTask = await _context.Tasks
                    .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .FirstAsync(t => t.TaskID == task.TaskID);

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
                    Message = "Error occurred while deleting task !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}