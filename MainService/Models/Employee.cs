using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService.Models;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }

    [Required]
    [MaxLength(50)]
    public string Subdivision { get; set; }

    [Required]
    [MaxLength(50)]
    public string Position { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; }

    [Required]
    public int PeoplePartnerId { get; set; }

    [Required]
    public int OutOfOfficeBalance { get; set; }

    [MaxLength(200)]
    public string PhotoPath { get; set; }

    [ForeignKey("PeoplePartnerId")]
    public Employee PeoplePartner { get; set; }

    public ICollection<LeaveRequest> LeaveRequests { get; set; }
    public ICollection<Project> ManagedProjects { get; set; }
    public ICollection<ProjectAssignment> ProjectAssignments { get; set; }
}
