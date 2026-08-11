using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace spm_backend.Models;

public class UserRole
{
    [Key]
    public int RolePermissionID { get; set; }
    
    [ForeignKey("RoleID")]
    [Required]
    public int RoleID { get; set; }
    public Role? Role { get; set; }
    
    [ForeignKey("UserID")]
    [Required]
    public int UserID { get; set; }
    public User? User { get; set; }
}