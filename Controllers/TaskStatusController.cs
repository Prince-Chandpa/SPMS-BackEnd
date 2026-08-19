using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
using spm_backend.Data;
using spm_backend.DTOs.TaskStatus;
using TaskStatus = spm_backend.Models.TaskStatus;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskStatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateTaskStatusDto> _createValidator;
        private readonly IValidator<UpdateTaskStatusDto> _updateValidator;

        public TaskStatusController(AppDbContext context, IValidator<CreateTaskStatusDto> createValidator, IValidator<UpdateTaskStatusDto> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.TaskStatuses.Select(ts => new TaskStatusDto
            {
                TaskStatusID = ts.TaskStatusID,
                TaskStatusName = ts.TaskStatusName,
                TaskStatusCssClass = ts.TaskStatusCssClass,
                IsActive = ts.IsActive
            }).ToListAsync();
            
            return Ok(new ApiResponse<List<TaskStatusDto>>
            {
                Success = true,
                Message = "Task Statuses Retrieved Successfully !!",
                Data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var taskStatus = await _context.TaskStatuses.FindAsync(id);
            
            if(taskStatus == null)
            { 
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Task Status Not Found !!",
                    Errors = new List<string> { $"No task status found with Id {id}" }
                });
            }
            
            var result = new TaskStatusDto
            {
                TaskStatusID = taskStatus.TaskStatusID,
                TaskStatusName = taskStatus.TaskStatusName,
                TaskStatusCssClass = taskStatus.TaskStatusCssClass,
                IsActive = taskStatus.IsActive
            };
            
            return Ok(new ApiResponse<TaskStatusDto>
            {
                Success = true,
                Message = "Task Status Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskStatusDto dto)
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
                
                var taskStatus = new TaskStatus
                {
                    TaskStatusName = dto.TaskStatusName,
                    TaskStatusCssClass = dto.TaskStatusCssClass,
                    IsActive = dto.IsActive
                };
                
                _context.TaskStatuses.Add(taskStatus);
                await _context.SaveChangesAsync();
                
                var result = new TaskStatusDto
                {
                    TaskStatusID = taskStatus.TaskStatusID,
                    TaskStatusName = taskStatus.TaskStatusName,
                    TaskStatusCssClass = taskStatus.TaskStatusCssClass,
                    IsActive = taskStatus.IsActive
                };

                return Ok(new ApiResponse<TaskStatusDto>
                {
                    Success = true,
                    Message = "Task Status Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating Task Status !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTaskStatusDto dto)
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
                
                var existingTaskStatus = await _context.TaskStatuses.FindAsync(id);

                if (existingTaskStatus == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Status Not Found !!",
                        Errors = new List<string> { $"No task status found with Id {id}" }
                    });
                }

                existingTaskStatus.TaskStatusName = dto.TaskStatusName;
                existingTaskStatus.TaskStatusCssClass = dto.TaskStatusCssClass;
                existingTaskStatus.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                var result = new TaskStatusDto
                {
                    TaskStatusID = existingTaskStatus.TaskStatusID,
                    TaskStatusName = existingTaskStatus.TaskStatusName,
                    TaskStatusCssClass = existingTaskStatus.TaskStatusCssClass,
                    IsActive = existingTaskStatus.IsActive
                };

                return Ok(new ApiResponse<TaskStatusDto>
                {
                    Success = true,
                    Message = "Task Status Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while updating task status !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var taskStatus = await _context.TaskStatuses.FindAsync(id);

                if (taskStatus == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Status Not Found !!"
                    });
                }
                
                _context.TaskStatuses.Remove(taskStatus);            
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Status Deleted Successfully !!",
                    Data = taskStatus
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting task status !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
