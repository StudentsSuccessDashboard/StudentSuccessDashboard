using StudentSuccessDashboard.Data;
using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class StudySession
    {
        public int StudySessionId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Range(1, 1440)]
        public int DurationMinutes { get; set; }

        [Required]
        [StringLength(200)]
        public string Topic { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Course Foreign Key
        public int CourseId { get; set; }

        // Course Navigation Property
        public Course Course { get; set; } = null!;

        // User Foreign Key
        [Required]
        public string UserId { get; set; } = string.Empty;

        // User Navigation Property
        public ApplicationUser User { get; set; } = null!;
    }
}