using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.UserType;

public class UpdateUserTypeDto
{
    [Required]
    [MaxLength(50)]
    public string UserTypeName { get; set; } = string.Empty;
    
    [MaxLength(250)]
    public string Description { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; }
}