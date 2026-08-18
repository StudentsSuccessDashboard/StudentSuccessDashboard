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
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return null;
            }

            return await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var courses = await _context.Courses
                .Include(c => c.Semester)
                .Where(c => c.StudentId == student.StudentId)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            return View(courses);
        }

        // GET: Courses/Details/5
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

            var course = await _context.Courses
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(c =>
                    c.CourseId == id &&
                    c.StudentId == student.StudentId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var semesters = await _context.Semesters
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            ViewData["SemesterId"] = new SelectList(
                semesters,
                "SemesterId",
                "SemesterName"
            );

            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            course.StudentId = student.StudentId;

            var semesterBelongsToStudent =
                await _context.Semesters.AnyAsync(s =>
                    s.SemesterId == course.SemesterId &&
                    s.StudentId == student.StudentId);

            if (!semesterBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Course.SemesterId),
                    "Please select one of your semesters."
                );
            }

            ModelState.Remove(nameof(Course.Student));
            ModelState.Remove(nameof(Course.StudentId));
            ModelState.Remove(nameof(Course.Semester));

            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var semesters = await _context.Semesters
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            ViewData["SemesterId"] = new SelectList(
                semesters,
                "SemesterId",
                "SemesterName",
                course.SemesterId
            );

            return View(course);
        }

        // GET: Courses/Edit/5
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

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.CourseId == id &&
                    c.StudentId == student.StudentId);

            if (course == null)
            {
                return NotFound();
            }

            var semesters = await _context.Semesters
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            ViewData["SemesterId"] = new SelectList(
                semesters,
                "SemesterId",
                "SemesterName",
                course.SemesterId
            );

            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var existingCourse = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.CourseId == id &&
                    c.StudentId == student.StudentId);

            if (existingCourse == null)
            {
                return NotFound();
            }

            var semesterBelongsToStudent =
                await _context.Semesters.AnyAsync(s =>
                    s.SemesterId == course.SemesterId &&
                    s.StudentId == student.StudentId);

            if (!semesterBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Course.SemesterId),
                    "Please select one of your semesters."
                );
            }

            ModelState.Remove(nameof(Course.Student));
            ModelState.Remove(nameof(Course.StudentId));
            ModelState.Remove(nameof(Course.Semester));

            if (ModelState.IsValid)
            {
                existingCourse.CourseName = course.CourseName;
                existingCourse.CourseCode = course.CourseCode;
                existingCourse.Instructor = course.Instructor;
                existingCourse.Credits = course.Credits;
                existingCourse.SemesterId = course.SemesterId;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var semesters = await _context.Semesters
                .Where(s => s.StudentId == student.StudentId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            ViewData["SemesterId"] = new SelectList(
                semesters,
                "SemesterId",
                "SemesterName",
                course.SemesterId
            );

            return View(course);
        }

        // GET: Courses/Delete/5
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

            var course = await _context.Courses
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(c =>
                    c.CourseId == id &&
                    c.StudentId == student.StudentId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.CourseId == id &&
                    c.StudentId == student.StudentId);

            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}