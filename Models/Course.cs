using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; } = "";

        [Required]
        [StringLength(20)]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Instructor { get; set; } = "";

        [Range(1, 6)]
        public int Credits { get; set; }

        // Foreign Key
        public int StudentId { get; set; }

        // Navigation Property
        public Student Student { get; set; } = null!;

        // Semester Relationship
        public int SemesterId { get; set; }

        public Semester Semester { get; set; } = null!;

        // Navigation Property
        public ICollection<Assignment> Assignments { get; set; }
            = new List<Assignment>();

        // Navigation Property
        public ICollection<GradeRecord> Grades { get; set; }
            = new List<GradeRecord>();

        // Navigation Property
        public ICollection<Quiz> Quizzes { get; set; }
            = new List<Quiz>();

        // Navigation Property
        public ICollection<Exam> Exams { get; set; }
            = new List<Exam>();

        // Navigation Property
        public ICollection<StudySession> StudySessions { get; set; }
            = new List<StudySession>();
    }
}