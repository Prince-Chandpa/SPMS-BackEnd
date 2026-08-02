using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;

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
            
            return Ok(taskPriorities);
        }
    }
}