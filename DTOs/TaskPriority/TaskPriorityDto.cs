namespace spm_backend.DTOs.TaskPriority;

public class TaskPriorityDto
{
    public int TaskPriorityID { get; set; }
    
    public string TaskPriorityName { get; set; } = string.Empty;
    
    public string TaskPriorityCssClass { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; }
}