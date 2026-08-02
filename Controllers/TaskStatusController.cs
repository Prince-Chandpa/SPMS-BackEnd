using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;

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
            
            return Ok(taskStatuses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var taskStatus = await _context.TaskStatuses.FirstOrDefaultAsync(s => s.TaskStatusID == id);
            if(taskStatus == null) return NotFound("Task Status Not Found!!");
            return Ok(taskStatus);
        }

        
    }
}
