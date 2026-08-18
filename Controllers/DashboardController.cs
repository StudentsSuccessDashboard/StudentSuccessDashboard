using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Data;

namespace StudentSuccessDashboard.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

            var upcomingAssignments =
                await _context.Assignments
                    .Include(a => a.Course)
                    .Where(a =>
                        a.Course.StudentId == student.StudentId &&
                        a.DueDate >= today &&
                        !a.Completed)
                    .OrderBy(a => a.DueDate)
                    .Take(5)
                    .ToListAsync();

            var upcomingQuizzes =
                await _context.Quizzes
                    .Include(q => q.Course)
                    .Where(q =>
                        q.Course.StudentId == student.StudentId &&
                        q.DueDate >= today)
                    .OrderBy(q => q.DueDate)
                    .Take(5)
                    .ToListAsync();

            var upcomingExams =
                await _context.Exams
                    .Include(e => e.Course)
                    .Where(e =>
                        e.Course.StudentId == student.StudentId &&
                        e.ExamDate >= today)
                    .OrderBy(e => e.ExamDate)
                    .Take(5)
                    .ToListAsync();

            ViewBag.UpcomingAssignments = upcomingAssignments;
            ViewBag.UpcomingQuizzes = upcomingQuizzes;
            ViewBag.UpcomingExams = upcomingExams;

            return View();
        }
    }
}