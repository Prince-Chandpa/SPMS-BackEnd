namespace spm_backend.DTOs.Task;

public class TaskDto
{
    public int TaskID {get; set;}
    
    public int ProjectAllocationID { get; set; }

    public string ProjectTitle { get; set; } = string.Empty;
    
    public int TaskStatusID { get; set; }
    
    public string TaskStatusName { get; set; } = string.Empty;
    
    public int TaskPriorityID { get; set; }
    
    public string TaskPriorityName { get; set; } = string.Empty;
    
    public string TaskTitle { get; set; } = string.Empty;
    
    public string TaskDescription { get; set; } = string.Empty;
    
    public decimal AssignedScore { get; set; }
    
    public decimal? EarnedScore { get; set; }
    
    public decimal ProgressPercentage {get; set;}
    
    public DateTime TaskAssignedDate { get; set; }
    
    public DateTime? TaskStartDate { get; set; }
    
    public DateTime? TaskDueDate { get; set; }
    
    public DateTime? TaskCompletedDate { get; set; }
    
    public DateTime? NextFollowUpDate { get; set; }
    
    public string FacultyRemarks {get; set;} = string.Empty;
    
    public string StudentRemarks {get; set;} = string.Empty;
    
    public bool IsActive { get; set; }
}