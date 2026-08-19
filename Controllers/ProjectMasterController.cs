using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Common;
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
        private readonly IValidator<CreateProjectMasterDto> _createValidator;
        private readonly IValidator<UpdateProjectMasterDto> _updateValidator;
        
        public ProjectMasterController(AppDbContext context,  IValidator<CreateProjectMasterDto> createValidator, IValidator<UpdateProjectMasterDto> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.ProjectMasters.Select(pm => new ProjectMasterDto
            {
                ProjectMasterID = pm.ProjectMasterID,
                ProjectTitle = pm.ProjectTitle,
                Description = pm.Description,
                IsActive = pm.IsActive
            }).ToListAsync();
            
            return Ok(new ApiResponse<List<ProjectMasterDto>>
            {
                Success = true,
                Message = "Project Master Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var projectMaster = await _context.ProjectMasters.FindAsync(id);
            
            if(projectMaster == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Project Master Not Found !!",
                    Errors = new List<string> { $"No project master found with Id {id}" }
                });
            }
            
            var result = new ProjectMasterDto
            {
                ProjectMasterID = projectMaster.ProjectMasterID,
                ProjectTitle = projectMaster.ProjectTitle,
                Description = projectMaster.Description,
                IsActive = projectMaster.IsActive
            };
            
            return Ok(new ApiResponse<ProjectMasterDto>
            {
                Success = true,
                Message = "Project Master Retrieved Successfully !!",
                Data = result
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectMasterDto dto)
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

                return Ok(new ApiResponse<ProjectMasterDto>
                {
                    Success = true,
                    Message = "Project Master Created Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while creating Project Master !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProjectMasterDto dto)
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
                
                var existingProjectMaster = await _context.ProjectMasters.FindAsync(id);

                if (existingProjectMaster == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Master Not Found !!",
                        Errors = new List<string> { $"No project master found with Id {id}" }
                    });
                }

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

                return Ok(new ApiResponse<ProjectMasterDto>
                {
                    Success = true,
                    Message = "Project Master Updated Successfully !!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ProjectMasterDto>
                {
                    Success = false,
                    Message = "Error occurred while updating project master !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var projectMaster = await _context.ProjectMasters.FindAsync(id);

                if (projectMaster == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Master Not Found !!",
                    });
                }

                _context.ProjectMasters.Remove(projectMaster);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Master Deleted Successfully !!",
                    Data = projectMaster
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting project master !!",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}

