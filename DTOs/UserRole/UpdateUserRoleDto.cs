using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.UserRole;

public class UpdateUserRoleDto
{
    [Required]
    public int RoleID { get; set; }
    
    [Required]
    public int UserID { get; set; }
}