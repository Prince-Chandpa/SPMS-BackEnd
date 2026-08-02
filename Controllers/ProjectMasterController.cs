using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMasterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectMasterController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projectMasters = await _context.ProjectMasters.ToListAsync();
            
            return Ok(projectMasters);
        }
    }
}

