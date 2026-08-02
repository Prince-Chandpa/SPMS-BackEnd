using Microsoft.EntityFrameworkCore;
using spm_backend.Models;
using Task = spm_backend.Models.Task;
using TaskStatus = spm_backend.Models.TaskStatus;

namespace spm_backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
    }
    
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<TaskStatus> TaskStatuses => Set<TaskStatus>();
    public DbSet<TaskPriority> TaskPriorities => Set<TaskPriority>();
    public DbSet<ProjectMaster>  ProjectMasters => Set<ProjectMaster>();
    public DbSet<ProjectAllocation> ProjectAllocations => Set<ProjectAllocation>();
    public DbSet<Task> Tasks => Set<Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserType)
            .WithMany(u => u.Users)
            .HasForeignKey(u => u.UserTypeID);
        
        modelBuilder.Entity<ProjectAllocation>()
            .HasOne(pa => pa.ProjectMaster)
            .WithMany()
            .HasForeignKey(pa => pa.ProjectID);

        modelBuilder.Entity<ProjectAllocation>()
            .HasOne(pa => pa.UserStudent)
            .WithMany()
            .HasForeignKey(pa => pa.StudentID)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<ProjectAllocation>()
            .HasOne(pa => pa.UserFaculty)
            .WithMany()
            .HasForeignKey(pa => pa.FacultyID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserID);
        
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleID);
    }
}