using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Semester
    {
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Semester name is required.")]
        [StringLength(50)]
        [Display(Name = "Semester Name")]
        public string SemesterName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        // Foreign Key to Student
        public int StudentId { get; set; }

        // Navigation Property
        public Student Student { get; set; } = null!;

        // Navigation Property
        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}