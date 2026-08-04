using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium";

        [Required]
        public string Status { get; set; } = "Not Started";

        [Range(0, double.MaxValue)]
        public double PointsPossible { get; set; }

        public bool Completed { get; set; }

        public string Notes { get; set; } = string.Empty;

        // Foreign Key
        public int CourseId { get; set; }

        // Navigation Property
        public Course Course { get; set; } = null!;
    }
}