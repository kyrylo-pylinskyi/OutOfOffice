using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService.Models;

public class ApprovalRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ApproverId { get; set; }

    [Required]
    public int LeaveRequestId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; }

    public string Comment { get; set; }

    [ForeignKey("ApproverId")]
    public Employee Approver { get; set; }

    [ForeignKey("LeaveRequestId")]
    public LeaveRequest LeaveRequest { get; set; }
}
