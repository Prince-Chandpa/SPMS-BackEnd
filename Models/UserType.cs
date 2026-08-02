using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace spm_backend.Models;

public class UserType
{
    [Key]
    public int UserTypeID { get; set; }
    
    [Required,MaxLength(50)]
    public string UserTypeName { get; set; } = string.Empty;
    
    [MaxLength(250)]
    public string Description { get; set; } =  string.Empty;
    
    [JsonIgnore]
    public ICollection<User> Users { get; set; } = new List<User>();
}