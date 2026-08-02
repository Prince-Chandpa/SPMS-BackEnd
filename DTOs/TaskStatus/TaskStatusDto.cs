namespace spm_backend.DTOs.TaskStatus;

public class TaskStatusDto
{
    public int TaskStatusID { get; set; }
    
    public string TaskStatusName { get; set; } = string.Empty;
    
    public string TaskStatusCssClass { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; }
}