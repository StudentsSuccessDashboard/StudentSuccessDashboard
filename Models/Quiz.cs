using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Quiz Name")]
        public string QuizName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Range(0, 100)]
        [Display(Name = "Points Possible")]
        public int PointsPossible { get; set; }


        // Course Relationship
        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;
    }
}