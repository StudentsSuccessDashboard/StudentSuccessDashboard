using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Data;
using StudentSuccessDashboard.Models;

namespace StudentSuccessDashboard.Controllers
{
    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizzesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (userId == null)
            {
                return null;
            }

            return await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IActionResult> Index()
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var quizzes = await _context.Quizzes
                .Include(q => q.Course)
                .Where(q =>
                    q.Course.StudentId == student.StudentId)
                .OrderBy(q => q.DueDate)
                .ToListAsync();

            return View(quizzes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == id &&
                    q.Course.StudentId == student.StudentId);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        public async Task<IActionResult> Create()
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var courses = await _context.Courses
                .Where(c =>
                    c.StudentId == student.StudentId)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            ViewData["CourseId"] = new SelectList(
                courses,
                "CourseId",
                "CourseName"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var courseBelongsToStudent =
                await _context.Courses.AnyAsync(c =>
                    c.CourseId == quiz.CourseId &&
                    c.StudentId == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Quiz.CourseId),
                    "Please select one of your courses."
                );
            }

            quiz.DueDate = DateTime.SpecifyKind(
                quiz.DueDate,
                DateTimeKind.Utc
            );

            ModelState.Remove(nameof(Quiz.Course));

            if (ModelState.IsValid)
            {
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var courses = await _context.Courses
                .Where(c =>
                    c.StudentId == student.StudentId)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            ViewData["CourseId"] = new SelectList(
                courses,
                "CourseId",
                "CourseName",
                quiz.CourseId
            );

            return View(quiz);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == id &&
                    q.Course.StudentId == student.StudentId);

            if (quiz == null)
            {
                return NotFound();
            }

            var courses = await _context.Courses
                .Where(c =>
                    c.StudentId == student.StudentId)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            ViewData["CourseId"] = new SelectList(
                courses,
                "CourseId",
                "CourseName",
                quiz.CourseId
            );

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Quiz quiz)
        {
            if (id != quiz.QuizId)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var existingQuiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == id &&
                    q.Course.StudentId == student.StudentId);

            if (existingQuiz == null)
            {
                return NotFound();
            }

            var courseBelongsToStudent =
                await _context.Courses.AnyAsync(c =>
                    c.CourseId == quiz.CourseId &&
                    c.StudentId == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Quiz.CourseId),
                    "Please select one of your courses."
                );
            }

            ModelState.Remove(nameof(Quiz.Course));

            if (ModelState.IsValid)
            {
                existingQuiz.QuizName =
                    quiz.QuizName;

                existingQuiz.DueDate =
                    DateTime.SpecifyKind(
                        quiz.DueDate,
                        DateTimeKind.Utc
                    );

                existingQuiz.PointsPossible =
                    quiz.PointsPossible;

                existingQuiz.CourseId =
                    quiz.CourseId;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var courses = await _context.Courses
                .Where(c =>
                    c.StudentId == student.StudentId)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            ViewData["CourseId"] = new SelectList(
                courses,
                "CourseId",
                "CourseName",
                quiz.CourseId
            );

            return View(quiz);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == id &&
                    q.Course.StudentId == student.StudentId);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q =>
                    q.QuizId == id &&
                    q.Course.StudentId == student.StudentId);

            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}