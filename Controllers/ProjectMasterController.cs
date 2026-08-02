using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.ProjectMaster;
using spm_backend.Models;

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

            var result = projectMasters.Select(pm => new ProjectMasterDto
            {
                ProjectMasterID = pm.ProjectMasterID,
                ProjectTitle = pm.ProjectTitle,
                Description = pm.Description,
                IsActive = pm.IsActive
            });
            
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var projectMaster = await _context.ProjectMasters.FindAsync(id);
            
            if(projectMaster == null) return NotFound("Project Master Not Found!!");
            
            var result = new ProjectMasterDto
            {
                ProjectMasterID = projectMaster.ProjectMasterID,
                ProjectTitle = projectMaster.ProjectTitle,
                Description = projectMaster.Description,
                IsActive = projectMaster.IsActive
            };
            
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectMasterDto dto)
        {
            var projectMaster = new ProjectMaster
            {
                ProjectTitle = dto.ProjectTitle,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.ProjectMasters.Add(projectMaster);
            await _context.SaveChangesAsync();

            var result = new ProjectMasterDto
            {
                ProjectMasterID = projectMaster.ProjectMasterID,
                ProjectTitle = projectMaster.ProjectTitle,
                Description = projectMaster.Description,
                IsActive = projectMaster.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = result.ProjectMasterID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectMasterDto dto)
        {
            var existingProjectMaster = await _context.ProjectMasters.FindAsync(id);

            if (existingProjectMaster == null)
                return NotFound("Project Master Not Found !!");

            existingProjectMaster.ProjectTitle = dto.ProjectTitle;
            existingProjectMaster.Description = dto.Description;
            existingProjectMaster.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            var result = new ProjectMasterDto
            {
                ProjectMasterID = existingProjectMaster.ProjectMasterID,
                ProjectTitle = existingProjectMaster.ProjectTitle,
                Description = existingProjectMaster.Description,
                IsActive = existingProjectMaster.IsActive
            };
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var projectMaster = await _context.ProjectMasters.FindAsync(id);

            if (projectMaster == null)
                return NotFound("Project Master Not Found !!");
            
            _context.ProjectMasters.Remove(projectMaster);            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

