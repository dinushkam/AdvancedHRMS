using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedHRMS.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public string Status { get; set; }

        public double HoursWorked
        {
            get
            {
                if (CheckInTime != null && CheckOutTime != null)
                    return Math.Round((CheckOutTime.Value - CheckInTime.Value).TotalHours, 2);
                return 0;
            }
        }
    }
}
