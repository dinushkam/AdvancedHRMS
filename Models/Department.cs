using System.ComponentModel.DataAnnotations;

namespace AdvancedHRMS.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
