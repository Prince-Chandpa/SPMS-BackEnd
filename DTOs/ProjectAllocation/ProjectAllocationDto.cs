namespace spm_backend.DTOs.ProjectAllocation;

public class ProjectAllocationDto
{
    public int ProjectAllocationID { get; set; }
    
    public int ProjectID { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    
    public int StudentID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    
    public int FacultyID { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    
    public DateTime AssignedDate { get; set; }
    
    public DateTime ProjectStartDate { get; set; }
    
    public DateTime ProjectEndDate { get; set; }
    
    public int TotalTasksGiven {get; set;}
    
    public int TotalCompletedTasks {get; set;}
    
    public decimal ProgressPercentage {get; set;}
    
    public string OverAllGrade { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}