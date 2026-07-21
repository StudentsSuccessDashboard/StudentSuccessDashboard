using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Course
    {
        public int CourseId { get; set; }


        [Required]
        public string CourseName { get; set; } = "";


        [Required]
        public string CourseCode { get; set; } = "";


        public string Instructor { get; set; } = "";


        public int Credits { get; set; }



        // Foreign Key
        public int StudentId { get; set; }



        // Navigation Property
        public Student Student { get; set; } = null!;


        public ICollection<Assignment> Assignments { get; set; }
            = new List<Assignment>();


        public ICollection<GradeRecord> Grades { get; set; }
            = new List<GradeRecord>();
    }
}