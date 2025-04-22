using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedHRMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }  // Add this line

        
        public virtual Employee Employee { get; set; }

        public bool IsApproved { get; set; } = false;
        public DateTime? LastCheckIn { get; set; }
        public DateTime? LastCheckOut { get; set; }
        public TimeSpan TotalHoursWorked { get; set; }
        public string? Department { get;  set; }
    }
}
