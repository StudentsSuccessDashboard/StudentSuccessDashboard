using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Data;
using StudentSuccessDashboard.Models;

namespace StudentSuccessDashboard.Controllers
{
    [Authorize]
    public class SemestersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SemestersController(ApplicationDbContext context)
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

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student != null)
            {
                return student;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return null;
            }

            student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student != null)
            {
                student.UserId = userId;
                await _context.SaveChangesAsync();

                return student;
            }

            student = new Student
            {
                FirstName = string.IsNullOrWhiteSpace(user.FirstName)
                    ? "Student"
                    : user.FirstName,

                LastName = string.IsNullOrWhiteSpace(user.LastName)
                    ? "User"
                    : user.LastName,

                Email = user.Email,
                UserId = userId,
                Major = "",
                GraduationYear = 2100
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return student;
        }

        public async Task<IActionResult> Index()
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

            return View(semesters);
        }

        public async Task<IActionResult> Create()
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semester semester)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            semester.StudentId = student.StudentId;

            semester.StartDate = DateTime.SpecifyKind(
                semester.StartDate,
                DateTimeKind.Utc
            );

            semester.EndDate = DateTime.SpecifyKind(
                semester.EndDate,
                DateTimeKind.Utc
            );

            ModelState.Remove(nameof(Semester.Student));
            ModelState.Remove(nameof(Semester.StudentId));

            if (ModelState.IsValid)
            {
                _context.Semesters.Add(semester);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(semester);
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

            var semester = await _context.Semesters
                .FirstOrDefaultAsync(s =>
                    s.SemesterId == id &&
                    s.StudentId == student.StudentId);

            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Semester semester)
        {
            if (id != semester.SemesterId)
            {
                return NotFound();
            }

            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var existingSemester = await _context.Semesters
                .FirstOrDefaultAsync(s =>
                    s.SemesterId == id &&
                    s.StudentId == student.StudentId);

            if (existingSemester == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Semester.Student));
            ModelState.Remove(nameof(Semester.StudentId));

            if (ModelState.IsValid)
            {
                existingSemester.SemesterName = semester.SemesterName;

                existingSemester.StartDate = DateTime.SpecifyKind(
                    semester.StartDate,
                    DateTimeKind.Utc
                );

                existingSemester.EndDate = DateTime.SpecifyKind(
                    semester.EndDate,
                    DateTimeKind.Utc
                );

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(semester);
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

            var semester = await _context.Semesters
                .FirstOrDefaultAsync(s =>
                    s.SemesterId == id &&
                    s.StudentId == student.StudentId);

            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var semester = await _context.Semesters
                .FirstOrDefaultAsync(s =>
                    s.SemesterId == id &&
                    s.StudentId == student.StudentId);

            if (semester == null)
            {
                return NotFound();
            }

            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}