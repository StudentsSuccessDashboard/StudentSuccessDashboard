using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class GradeRecord
    {
        public int GradeRecordId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Grade Category")]
        public string Category { get; set; } = "";

        [Range(0, 100)]
        public double Score { get; set; }

        [Range(0, 100)]
        public double Weight { get; set; }

        // Foreign Key
        public int CourseId { get; set; }

        // Navigation Property
        public Course? Course { get; set; }
    }
}