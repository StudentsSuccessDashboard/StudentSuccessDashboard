using System.ComponentModel.DataAnnotations;
using StudentSuccessDashboard.Data;

namespace StudentSuccessDashboard.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [StringLength(100)]
        public string? Major { get; set; }

        [Range(2000, 2100)]
        public int? GraduationYear { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public ICollection<Semester> Semesters { get; set; }
            = new List<Semester>();

        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}