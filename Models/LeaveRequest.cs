using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedHRMS.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string LeaveType { get; set; } // Sick, Vacation, Personal, etc.

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime RequestDate { get; set; } = DateTime.Now;

        [ForeignKey("ProcessedBy")]
        public int? ProcessedById { get; set; }
        public virtual User ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }
        public string? RejectionReason { get; set; }
        [NotMapped] // This property is calculated, not stored in DB
        public int Days => (EndDate - StartDate).Days + 1;
    }
}