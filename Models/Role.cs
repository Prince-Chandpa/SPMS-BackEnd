using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace spm_backend.Models;

public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [ MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;
    
    [JsonIgnore]
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}