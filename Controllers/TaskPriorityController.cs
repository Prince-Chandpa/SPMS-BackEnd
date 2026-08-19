using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
using spm_backend.Data;
using spm_backend.DTOs.TaskPriority;
using spm_backend.Models;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskPriorityController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateTaskPriorityDto> _createValidator;
        private readonly IValidator<UpdateTaskPriorityDto> _updateValidator;

        public TaskPriorityController(AppDbContext context, IValidator<CreateTaskPriorityDto> createValidator, IValidator<UpdateTaskPriorityDto> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.TaskPriorities.Select(tp => new TaskPriorityDto
            {
                TaskPriorityID = tp.TaskPriorityID,
                TaskPriorityName = tp.TaskPriorityName,
                TaskPriorityCssClass = tp.TaskPriorityCssClass,
                IsActive = tp.IsActive
            }).ToListAsync();
            
            return Ok(new ApiResponse<List<TaskPriorityDto>>
            {
                Success = true,
                Message = "Task Priorities Retrieved Successfully !!",
                Data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var taskPriority = await _context.TaskPriorities.FindAsync(id);

            if (taskPriority == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Task Priority Not Found !!",
                    Errors = new List<string> { $"No task priority found with Id {id}" }
                });
            }

            var result = new TaskPriorityDto
            {
                TaskPriorityID = taskPriority.TaskPriorityID,
                TaskPriorityName = taskPriority.TaskPriorityName,
                TaskPriorityCssClass = taskPriority.TaskPriorityCssClass,
                IsActive = taskPriority.IsActive
            };

            return Ok(new ApiResponse<TaskPriorityDto>
            {
                Success = true,
                Message = "Task Priority Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskPriorityDto dto)
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
                
                var taskPriority = new TaskPriority
                {
                    TaskPriorityName = dto.TaskPriorityName,
                    TaskPriorityCssClass = dto.TaskPriorityCssClass,
                    IsActive = dto.IsActive
                };

                _context.TaskPriorities.Add(taskPriority);
                await _context.SaveChangesAsync();

                var result = new TaskPriorityDto
                {
                    TaskPriorityID = taskPriority.TaskPriorityID,
                    TaskPriorityName = taskPriority.TaskPriorityName,
                    TaskPriorityCssClass = taskPriority.TaskPriorityCssClass,
                    IsActive = taskPriority.IsActive
                };

                return Ok(new ApiResponse<TaskPriorityDto>
                {
                    Success = true,
                    Message = "Task Priority Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating Task Priority !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTaskPriorityDto dto)
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
                
                var existingTaskPriority = await _context.TaskPriorities.FindAsync(id);

                if (existingTaskPriority == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Priority Not Found !!",
                        Errors = new List<string> { $"No task priority found with Id {id}" }
                    });
                }

                existingTaskPriority.TaskPriorityName = dto.TaskPriorityName;
                existingTaskPriority.TaskPriorityCssClass = dto.TaskPriorityCssClass;
                existingTaskPriority.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                var result = new TaskPriorityDto
                {
                    TaskPriorityID = existingTaskPriority.TaskPriorityID,
                    TaskPriorityName = existingTaskPriority.TaskPriorityName,
                    TaskPriorityCssClass = existingTaskPriority.TaskPriorityCssClass,
                    IsActive = existingTaskPriority.IsActive
                };
            
                return Ok(new ApiResponse<TaskPriorityDto>
                {
                    Success = true,
                    Message = "Task Priority Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while updating task priority !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var taskPriority = await _context.TaskPriorities.FindAsync(id);

                if (taskPriority == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Priority Not Found !!"
                    });
                }
                
                _context.TaskPriorities.Remove(taskPriority);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Priority Deleted Successfully !!",
                    Data = taskPriority
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting task priority !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}