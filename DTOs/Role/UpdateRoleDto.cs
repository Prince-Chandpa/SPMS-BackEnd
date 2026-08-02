using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.Role;

public class UpdateRoleDto
{
    [Required]
    [MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}