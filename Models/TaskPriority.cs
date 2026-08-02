using System.ComponentModel.DataAnnotations;

namespace spm_backend.Models;

public class TaskPriority
{
    [Key]
    public int TaskPriorityID { get; set; }
    
    [Required,MaxLength(20)]
    public string TaskPriorityName { get; set; } =  string.Empty;
    
    [Required,MaxLength(20)]
    public string TaskPriorityCssClass { get; set; } =  string.Empty;
}