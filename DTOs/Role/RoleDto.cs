namespace spm_backend.DTOs.Role;

public class RoleDto
{
    public int RoleID { get; set; }
    
    public string RoleName { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
}