using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Student
    {
        public int StudentId { get; set; }


        [Required]
        public string FirstName { get; set; } = "";


        [Required]
        public string LastName { get; set; } = "";


        [Required]
        public string Email { get; set; } = "";


        public string Major { get; set; } = "";


        public int GraduationYear { get; set; }



        // Navigation Property
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}