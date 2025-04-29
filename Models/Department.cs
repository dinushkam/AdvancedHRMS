using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedHRMS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Department head relationship
        [ForeignKey("Manager")]
        public int? ManagerId { get; set; }
        public virtual Employee Manager { get; set; }

        // Navigation properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        [NotMapped]
        public int EmployeeCount => Employees?.Count ?? 0;

        // Budget information
        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; }

        public DateTime EstablishedDate { get; set; } = DateTime.Now;
    }
}