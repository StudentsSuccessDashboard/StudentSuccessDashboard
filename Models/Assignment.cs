using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }


        [Required]
        public string Title { get; set; } = "";


        public DateTime DueDate { get; set; }


        public string Priority { get; set; } = "";


        public bool Completed { get; set; }


        public string Notes { get; set; } = "";



        // Foreign Key
        public int CourseId { get; set; }



        // Navigation Property
        public Course Course { get; set; } = null!;
    }
}