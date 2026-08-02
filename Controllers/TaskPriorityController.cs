using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public TaskPriorityController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var taskPriorities = await _context.TaskPriorities.ToListAsync();
            
            var result = taskPriorities.Select(tp => new TaskPriorityDto
            {
                TaskPriorityID = tp.TaskPriorityID,
                TaskPriorityName = tp.TaskPriorityName,
                TaskPriorityCssClass = tp.TaskPriorityCssClass,
                IsActive = tp.IsActive
            });
            
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var taskPriority = await _context.TaskPriorities.FindAsync(id);
            
            if(taskPriority == null) return NotFound("Task Priority Not Found!!");
            
            var result = new TaskPriorityDto
            {
                TaskPriorityID = taskPriority.TaskPriorityID,
                TaskPriorityName = taskPriority.TaskPriorityName,
                TaskPriorityCssClass = taskPriority.TaskPriorityCssClass,
                IsActive = taskPriority.IsActive
            };
            
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskPriorityDto dto)
        {
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

            return CreatedAtAction(nameof(GetById), new { id = result.TaskPriorityID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskPriorityDto dto)
        {
            var existingTaskPriority = await _context.TaskPriorities.FindAsync(id);

            if (existingTaskPriority == null)
                return NotFound("Task Priority Not Found !!");

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
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskPriority = await _context.TaskPriorities.FindAsync(id);

            if (taskPriority == null)
                return NotFound("Task Priority Not Found !!");
            
            _context.TaskPriorities.Remove(taskPriority);            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}