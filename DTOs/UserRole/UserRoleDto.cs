namespace spm_backend.DTOs.UserRole;

public class UserRoleDto
{
    public int RolePermissionID { get; set; }
    
    public int RoleID { get; set; }

    public string RoleName { get; set; } = string.Empty;
    
    public int UserID { get; set; }
    
    public string UserName { get; set; } = string.Empty;
}