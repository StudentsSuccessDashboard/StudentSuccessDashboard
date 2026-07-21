using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class GradeRecord
    {
        public int GradeRecordId { get; set; }


        public string Category { get; set; } = "";


        public double Score { get; set; }


        public double Weight { get; set; }



        // Foreign Key
        public int CourseId { get; set; }



        // Navigation Property
        public Course Course { get; set; } = null!;
    }
}