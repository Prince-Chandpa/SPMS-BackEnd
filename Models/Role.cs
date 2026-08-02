using System.ComponentModel.DataAnnotations;

namespace spm_backend.Models;

public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required, MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Description { get; set; } = string.Empty;
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}