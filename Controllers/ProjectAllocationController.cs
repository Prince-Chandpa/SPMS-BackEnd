using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;

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
            var projectAllocations = await _context.ProjectAllocations.ToListAsync();
            
            return Ok(projectAllocations);
        }
    }
}