using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Data;
using StudentSuccessDashboard.Services;

namespace StudentSuccessDashboard.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GradeCalculationService _gradeCalculationService;

        public DashboardController(
            ApplicationDbContext context,
            GradeCalculationService gradeCalculationService)
        {
            _context = context;
            _gradeCalculationService = gradeCalculationService;
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            int? courseId,
            string? priority)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (userId == null)
            {
                return Unauthorized();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return Unauthorized();
            }

            var today = DateTime.SpecifyKind(
                DateTime.UtcNow.Date,
                DateTimeKind.Utc
            );

            var daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;

            var weekStart =
                today.AddDays(-daysSinceMonday);

            var weekEnd =
                weekStart.AddDays(7);

            var studentCourses = await _context.Courses
                .Where(c => c.StudentId == student.StudentId)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            ViewBag.CourseFilter = new SelectList(
                studentCourses,
                "CourseId",
                "CourseName",
                courseId
            );

            ViewBag.SelectedCourseId = courseId;
            ViewBag.SelectedPriority = priority;

            var assignmentQuery = _context.Assignments
                .Include(a => a.Course)
                .Where(a =>
                    a.Course.StudentId == student.StudentId &&
                    a.DueDate >= today &&
                    !a.Completed);

            if (courseId.HasValue)
            {
                assignmentQuery = assignmentQuery
                    .Where(a => a.CourseId == courseId.Value);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                assignmentQuery = assignmentQuery
                    .Where(a => a.Priority == priority);
            }

            var upcomingAssignmentCount =
                await assignmentQuery.CountAsync();

            var upcomingAssignments = await assignmentQuery
                .OrderBy(a => a.DueDate)
                .Take(5)
                .ToListAsync();

            var overdueAssignments =
                await _context.Assignments
                    .Include(a => a.Course)
                    .Where(a =>
                        a.Course.StudentId == student.StudentId &&
                        a.DueDate < today &&
                        !a.Completed)
                    .OrderBy(a => a.DueDate)
                    .Take(5)
                    .ToListAsync();

            var totalAssignments =
                await _context.Assignments
                    .CountAsync(a =>
                        a.Course.StudentId == student.StudentId);

            var completedAssignments =
                await _context.Assignments
                    .CountAsync(a =>
                        a.Course.StudentId == student.StudentId &&
                        a.Completed);

            var assignmentCompletionPercentage =
                totalAssignments > 0
                    ? (double)completedAssignments
                        / totalAssignments * 100
                    : 0;

            var quizQuery = _context.Quizzes
                .Include(q => q.Course)
                .Where(q =>
                    q.Course.StudentId == student.StudentId &&
                    q.DueDate >= today);

            if (courseId.HasValue)
            {
                quizQuery = quizQuery
                    .Where(q => q.CourseId == courseId.Value);
            }

            var upcomingQuizCount =
                await quizQuery.CountAsync();

            var upcomingQuizzes = await quizQuery
                .OrderBy(q => q.DueDate)
                .Take(5)
                .ToListAsync();

            var examQuery = _context.Exams
                .Include(e => e.Course)
                .Where(e =>
                    e.Course.StudentId == student.StudentId &&
                    e.ExamDate >= today);

            if (courseId.HasValue)
            {
                examQuery = examQuery
                    .Where(e => e.CourseId == courseId.Value);
            }

            var upcomingExamCount =
                await examQuery.CountAsync();

            var upcomingExams = await examQuery
                .OrderBy(e => e.ExamDate)
                .Take(5)
                .ToListAsync();

            var upcomingDeadlineCount =
                upcomingAssignmentCount
                + upcomingQuizCount
                + upcomingExamCount;

            var gradeQuery = _context.GradeRecords
                .Include(g => g.Course)
                .Where(g =>
                    g.Course != null &&
                    g.Course.StudentId == student.StudentId);

            if (courseId.HasValue)
            {
                gradeQuery = gradeQuery
                    .Where(g => g.CourseId == courseId.Value);
            }

            var gradeRecords = await gradeQuery
                .ToListAsync();

            var currentGrades = gradeRecords
                .Where(g => g.Course != null)
                .GroupBy(g => g.Course!)
                .Select(group => new
                {
                    CourseName = group.Key.CourseName,
                    Average = _gradeCalculationService
                        .CalculateCourseAverage(group)
                })
                .OrderBy(g => g.CourseName)
                .ToList();

            var studyQuery = _context.StudySessions
                .Include(s => s.Course)
                .Where(s =>
                    s.UserId == userId &&
                    s.Course.StudentId == student.StudentId);

            if (courseId.HasValue)
            {
                studyQuery = studyQuery
                    .Where(s => s.CourseId == courseId.Value);
            }

            var weeklyStudyMinutes =
                await studyQuery
                    .Where(s =>
                        s.SessionDate >= weekStart &&
                        s.SessionDate < weekEnd)
                    .SumAsync(s => (int?)s.DurationMinutes)
                ?? 0;

            var recentStudySessions = await studyQuery
                .OrderByDescending(s => s.SessionDate)
                .Take(5)
                .ToListAsync();

            // Weekly mini calendar
            var weeklyAssignments = await _context.Assignments
                .Include(a => a.Course)
                .Where(a =>
                    a.Course.StudentId == student.StudentId &&
                    a.DueDate >= weekStart &&
                    a.DueDate < weekEnd)
                .OrderBy(a => a.DueDate)
                .ToListAsync();

            var weeklyQuizzes = await _context.Quizzes
                .Include(q => q.Course)
                .Where(q =>
                    q.Course.StudentId == student.StudentId &&
                    q.DueDate >= weekStart &&
                    q.DueDate < weekEnd)
                .OrderBy(q => q.DueDate)
                .ToListAsync();

            var weeklyExams = await _context.Exams
                .Include(e => e.Course)
                .Where(e =>
                    e.Course.StudentId == student.StudentId &&
                    e.ExamDate >= weekStart &&
                    e.ExamDate < weekEnd)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            var courseSearchResults = new List<object>();
            var assignmentSearchResults = new List<object>();
            var quizSearchResults = new List<object>();
            var examSearchResults = new List<object>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                var courseSearchQuery = _context.Courses
                    .Where(c =>
                        c.StudentId == student.StudentId &&
                        (
                            c.CourseName.ToLower().Contains(term) ||
                            c.CourseCode.ToLower().Contains(term) ||
                            c.Instructor.ToLower().Contains(term)
                        ));

                if (courseId.HasValue)
                {
                    courseSearchQuery = courseSearchQuery
                        .Where(c => c.CourseId == courseId.Value);
                }

                courseSearchResults = await courseSearchQuery
                    .Select(c => new
                    {
                        c.CourseId,
                        c.CourseName,
                        c.CourseCode,
                        c.Instructor
                    })
                    .Cast<object>()
                    .ToListAsync();

                var assignmentSearchQuery = _context.Assignments
                    .Include(a => a.Course)
                    .Where(a =>
                        a.Course.StudentId == student.StudentId &&
                        (
                            a.Title.ToLower().Contains(term) ||
                            a.Description.ToLower().Contains(term) ||
                            a.Priority.ToLower().Contains(term) ||
                            a.Status.ToLower().Contains(term)
                        ));

                if (courseId.HasValue)
                {
                    assignmentSearchQuery = assignmentSearchQuery
                        .Where(a => a.CourseId == courseId.Value);
                }

                if (!string.IsNullOrWhiteSpace(priority))
                {
                    assignmentSearchQuery = assignmentSearchQuery
                        .Where(a => a.Priority == priority);
                }

                assignmentSearchResults =
                    await assignmentSearchQuery
                        .Select(a => new
                        {
                            a.AssignmentId,
                            a.Title,
                            a.DueDate,
                            a.Priority,
                            CourseName = a.Course.CourseName
                        })
                        .Cast<object>()
                        .ToListAsync();

                var quizSearchQuery = _context.Quizzes
                    .Include(q => q.Course)
                    .Where(q =>
                        q.Course.StudentId == student.StudentId &&
                        q.QuizName.ToLower().Contains(term));

                if (courseId.HasValue)
                {
                    quizSearchQuery = quizSearchQuery
                        .Where(q => q.CourseId == courseId.Value);
                }

                quizSearchResults = await quizSearchQuery
                    .Select(q => new
                    {
                        q.QuizId,
                        q.QuizName,
                        q.DueDate,
                        CourseName = q.Course.CourseName
                    })
                    .Cast<object>()
                    .ToListAsync();

                var examSearchQuery = _context.Exams
                    .Include(e => e.Course)
                    .Where(e =>
                        e.Course.StudentId == student.StudentId &&
                        e.ExamName.ToLower().Contains(term));

                if (courseId.HasValue)
                {
                    examSearchQuery = examSearchQuery
                        .Where(e => e.CourseId == courseId.Value);
                }

                examSearchResults = await examSearchQuery
                    .Select(e => new
                    {
                        e.ExamId,
                        e.ExamName,
                        e.ExamDate,
                        CourseName = e.Course.CourseName
                    })
                    .Cast<object>()
                    .ToListAsync();
            }

            ViewBag.UpcomingAssignments = upcomingAssignments;
            ViewBag.OverdueAssignments = overdueAssignments;
            ViewBag.UpcomingQuizzes = upcomingQuizzes;
            ViewBag.TotalAssignments = totalAssignments;
            ViewBag.CompletedAssignments = completedAssignments;
            ViewBag.AssignmentCompletionPercentage =
                assignmentCompletionPercentage;
            ViewBag.UpcomingExams = upcomingExams;
            ViewBag.UpcomingDeadlineCount = upcomingDeadlineCount;
            ViewBag.CurrentGrades = currentGrades;
            ViewBag.RecentStudySessions = recentStudySessions;

            ViewBag.WeeklyStudyMinutes = weeklyStudyMinutes;
            ViewBag.WeekStart = weekStart;
            ViewBag.WeekEnd = weekEnd.AddDays(-1);

            ViewBag.WeeklyAssignments = weeklyAssignments;
            ViewBag.WeeklyQuizzes = weeklyQuizzes;
            ViewBag.WeeklyExams = weeklyExams;

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CourseSearchResults = courseSearchResults;
            ViewBag.AssignmentSearchResults = assignmentSearchResults;
            ViewBag.QuizSearchResults = quizSearchResults;
            ViewBag.ExamSearchResults = examSearchResults;

            return View();
        }
    }
}