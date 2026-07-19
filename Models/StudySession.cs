using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class StudySession
    {
        public int StudySessionId { get; set; }


        public DateTime SessionDate { get; set; }


        public int DurationMinutes { get; set; }


        public string Topic { get; set; } = "";


        public string Notes { get; set; } = "";



        // Foreign Key
        public int CourseId { get; set; }



        // Navigation Property
        public Course Course { get; set; } = null!;
    }
}