using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class Exam
    {
        public int ExamId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Exam Name")]
        public string ExamName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Exam Date")]
        public DateTime ExamDate { get; set; }

        [Range(0, 100)]
        [Display(Name = "Points Possible")]
        public int PointsPossible { get; set; }


        // Course Relationship
        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;
    }
}