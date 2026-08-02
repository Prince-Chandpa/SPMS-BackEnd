using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public TaskStatusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var taskStatuses = await _context.TaskStatuses.ToListAsync();

            var result = taskStatuses.Select(ts => new TaskStatusDto
            {
                TaskStatusID = ts.TaskStatusID,
                TaskStatusName = ts.TaskStatusName,
                TaskStatusCssClass = ts.TaskStatusCssClass,
                IsActive = ts.IsActive
            });
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var taskStatus = await _context.TaskStatuses.FindAsync(id);
            
            if(taskStatus == null) return NotFound("Task Status Not Found!!");
            
            var result = new TaskStatusDto
            {
                TaskStatusID = taskStatus.TaskStatusID,
                TaskStatusName = taskStatus.TaskStatusName,
                TaskStatusCssClass = taskStatus.TaskStatusCssClass,
                IsActive = taskStatus.IsActive
            };
            
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskStatusDto dto)
        {
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

            return CreatedAtAction(nameof(GetById), new { id = result.TaskStatusID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            var existingTaskStatus = await _context.TaskStatuses.FindAsync(id);

            if (existingTaskStatus == null)
                return NotFound("Task Status Not Found !!");

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
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskStatus = await _context.TaskStatuses.FindAsync(id);

            if (taskStatus == null)
                return NotFound("Task Status Not Found !!");
            
            _context.TaskStatuses.Remove(taskStatus);            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
