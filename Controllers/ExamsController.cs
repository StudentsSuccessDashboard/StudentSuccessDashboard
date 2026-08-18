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
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
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

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var exams = await _context.Exams
                .Include(e => e.Course)
                .Where(e =>
                    e.Course.StudentId == student.StudentId)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            return View(exams);
        }

        // GET: Exams/Details/5
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

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e =>
                    e.ExamId == id &&
                    e.Course.StudentId == student.StudentId);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Exams/Create
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

        // POST: Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var courseBelongsToStudent =
                await _context.Courses.AnyAsync(c =>
                    c.CourseId == exam.CourseId &&
                    c.StudentId == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Exam.CourseId),
                    "Please select one of your courses."
                );
            }

            exam.ExamDate = DateTime.SpecifyKind(
                exam.ExamDate,
                DateTimeKind.Utc
            );

            ModelState.Remove(nameof(Exam.Course));

            if (ModelState.IsValid)
            {
                _context.Exams.Add(exam);
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
                exam.CourseId
            );

            return View(exam);
        }

        // GET: Exams/Edit/5
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

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e =>
                    e.ExamId == id &&
                    e.Course.StudentId == student.StudentId);

            if (exam == null)
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
                exam.CourseId
            );

            return View(exam);
        }

        // POST: Exams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Exam exam)
        {
            if (id != exam.ExamId)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var existingExam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e =>
                    e.ExamId == id &&
                    e.Course.StudentId == student.StudentId);

            if (existingExam == null)
            {
                return NotFound();
            }

            var courseBelongsToStudent =
                await _context.Courses.AnyAsync(c =>
                    c.CourseId == exam.CourseId &&
                    c.StudentId == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Exam.CourseId),
                    "Please select one of your courses."
                );
            }

            ModelState.Remove(nameof(Exam.Course));

            if (ModelState.IsValid)
            {
                existingExam.ExamName =
                    exam.ExamName;

                existingExam.ExamDate =
                    DateTime.SpecifyKind(
                        exam.ExamDate,
                        DateTimeKind.Utc
                    );

                existingExam.PointsPossible =
                    exam.PointsPossible;

                existingExam.CourseId =
                    exam.CourseId;

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
                exam.CourseId
            );

            return View(exam);
        }

        // GET: Exams/Delete/5
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

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e =>
                    e.ExamId == id &&
                    e.Course.StudentId == student.StudentId);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // POST: Exams/Delete/5
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

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e =>
                    e.ExamId == id &&
                    e.Course.StudentId == student.StudentId);

            if (exam == null)
            {
                return NotFound();
            }

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}