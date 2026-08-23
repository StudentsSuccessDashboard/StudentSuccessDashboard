using StudentSuccessDashboard.Models;

namespace StudentSuccessDashboard.Services
{
    public class GradeCalculationService
    {
        /// <summary>
        /// Calculates the current weighted average for a course.
        /// Only grades that have been entered are included.
        /// </summary>
        public double CalculateCourseAverage(
            IEnumerable<GradeRecord> grades)
        {
            var gradeList = grades.ToList();

            if (!gradeList.Any())
            {
                return 0;
            }

            var totalWeight = gradeList.Sum(g => g.Weight);

            if (totalWeight <= 0)
            {
                return 0;
            }

            var weightedScore = gradeList.Sum(
                g => g.Score * g.Weight);

            return weightedScore / totalWeight;
        }

        /// <summary>
        /// Calculates the weighted contribution of a single grade.
        /// </summary>
        public double CalculateWeightedContribution(
            GradeRecord grade)
        {
            return grade.Score * (grade.Weight / 100.0);
        }

        /// <summary>
        /// Calculates a cumulative GPA using course credits.
        /// </summary>
        public double CalculateCumulativeGpa(
            IEnumerable<(double Average, int Credits)> courses)
        {
            var courseList = courses.ToList();

            if (!courseList.Any())
            {
                return 0;
            }

            var totalCredits = courseList.Sum(c => c.Credits);

            if (totalCredits <= 0)
            {
                return 0;
            }

            var totalQualityPoints = courseList.Sum(
                c => ConvertPercentageToGpa(c.Average) * c.Credits);

            return totalQualityPoints / totalCredits;
        }

        /// <summary>
        /// Converts a percentage course average to a 4.0 GPA value.
        /// </summary>
        public double ConvertPercentageToGpa(
            double percentage)
        {
            if (percentage >= 93)
                return 4.0;

            if (percentage >= 90)
                return 3.7;

            if (percentage >= 87)
                return 3.3;

            if (percentage >= 83)
                return 3.0;

            if (percentage >= 80)
                return 2.7;

            if (percentage >= 77)
                return 2.3;

            if (percentage >= 73)
                return 2.0;

            if (percentage >= 70)
                return 1.7;

            if (percentage >= 67)
                return 1.3;

            if (percentage >= 63)
                return 1.0;

            if (percentage >= 60)
                return 0.7;

            return 0.0;
        }
    }
}