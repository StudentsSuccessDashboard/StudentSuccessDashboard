using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Data;

namespace StudentSuccessDashboard.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? year, int? month)
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

            var today = DateTime.UtcNow;

            var selectedYear =
                year ?? today.Year;

            var selectedMonth =
                month ?? today.Month;

            if (selectedMonth < 1 || selectedMonth > 12)
            {
                selectedMonth = today.Month;
            }

            if (selectedYear < 1 || selectedYear > 9999)
            {
                selectedYear = today.Year;
            }

            var displayMonth = new DateTime(
                selectedYear,
                selectedMonth,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc
            );

            var nextMonth =
                displayMonth.AddMonths(1);

            var previousMonth =
                displayMonth.AddMonths(-1);

            // US5-3:
            // Retrieve assignment due dates for the
            // logged-in student's courses.
            var assignments = await _context.Assignments
                .Include(a => a.Course)
                .Where(a =>
                    a.Course.StudentId == student.StudentId &&
                    a.DueDate >= displayMonth &&
                    a.DueDate < nextMonth)
                .OrderBy(a => a.DueDate)
                .ToListAsync();

            // US5-4:
            // Retrieve quiz dates for the
            // logged-in student's courses.
            var quizzes = await _context.Quizzes
                .Include(q => q.Course)
                .Where(q =>
                    q.Course.StudentId == student.StudentId &&
                    q.DueDate >= displayMonth &&
                    q.DueDate < nextMonth)
                .OrderBy(q => q.DueDate)
                .ToListAsync();

            // US5-4:
            // Retrieve exam dates for the
            // logged-in student's courses.
            var exams = await _context.Exams
                .Include(e => e.Course)
                .Where(e =>
                    e.Course.StudentId == student.StudentId &&
                    e.ExamDate >= displayMonth &&
                    e.ExamDate < nextMonth)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            ViewBag.DisplayMonth = displayMonth;

            ViewBag.PreviousYear =
                previousMonth.Year;

            ViewBag.PreviousMonth =
                previousMonth.Month;

            ViewBag.NextYear =
                nextMonth.Year;

            ViewBag.NextMonth =
                nextMonth.Month;

            ViewBag.Assignments = assignments;
            ViewBag.Quizzes = quizzes;
            ViewBag.Exams = exams;

            return View();
        }
    }
}