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
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(
            ApplicationDbContext context)
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
                .FirstOrDefaultAsync(
                    s => s.UserId == userId
                );
        }

        private bool IsAssignmentCompleted(string? status)
        {
            return status == "Completed"
                || status == "Submitted";
        }


        // GET: Assignments
        public async Task<IActionResult> Index()
        {
            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var assignments =
                await _context.Assignments
                    .Include(a => a.Course)
                    .Where(a =>
                        a.Course.StudentId
                        == student.StudentId)
                    .OrderBy(a => a.DueDate)
                    .ToListAsync();

            return View(assignments);
        }


        // GET: Assignments/Details/5
        public async Task<IActionResult> Details(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var assignment =
                await _context.Assignments
                    .Include(a => a.Course)
                    .FirstOrDefaultAsync(a =>
                        a.AssignmentId == id
                        &&
                        a.Course.StudentId
                        == student.StudentId);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }


        // GET: Assignments/Create
        public async Task<IActionResult> Create()
        {
            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var courses =
                await _context.Courses
                    .Where(c =>
                        c.StudentId
                        == student.StudentId)
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();

            ViewData["CourseId"] =
                new SelectList(
                    courses,
                    "CourseId",
                    "CourseName"
                );

            return View();
        }


        // POST: Assignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Assignment assignment)
        {
            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var courseBelongsToStudent =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseId
                        == assignment.CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Assignment.CourseId),
                    "Please select one of your courses."
                );
            }

            assignment.DueDate =
                DateTime.SpecifyKind(
                    assignment.DueDate,
                    DateTimeKind.Utc
                );

            assignment.Completed =
                IsAssignmentCompleted(
                    assignment.Status
                );

            ModelState.Remove(
                nameof(Assignment.Course)
            );

            if (ModelState.IsValid)
            {
                _context.Assignments.Add(
                    assignment
                );

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index)
                );
            }

            var courses =
                await _context.Courses
                    .Where(c =>
                        c.StudentId
                        == student.StudentId)
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();

            ViewData["CourseId"] =
                new SelectList(
                    courses,
                    "CourseId",
                    "CourseName",
                    assignment.CourseId
                );

            return View(assignment);
        }


        // GET: Assignments/Edit/5
        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var assignment =
                await _context.Assignments
                    .Include(a => a.Course)
                    .FirstOrDefaultAsync(a =>
                        a.AssignmentId == id
                        &&
                        a.Course.StudentId
                        == student.StudentId);

            if (assignment == null)
            {
                return NotFound();
            }

            var courses =
                await _context.Courses
                    .Where(c =>
                        c.StudentId
                        == student.StudentId)
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();

            ViewData["CourseId"] =
                new SelectList(
                    courses,
                    "CourseId",
                    "CourseName",
                    assignment.CourseId
                );

            return View(assignment);
        }


        // POST: Assignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Assignment assignment)
        {
            if (id != assignment.AssignmentId)
            {
                return NotFound();
            }

            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var existingAssignment =
                await _context.Assignments
                    .Include(a => a.Course)
                    .FirstOrDefaultAsync(a =>
                        a.AssignmentId == id
                        &&
                        a.Course.StudentId
                        == student.StudentId);

            if (existingAssignment == null)
            {
                return NotFound();
            }

            var courseBelongsToStudent =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseId
                        == assignment.CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(Assignment.CourseId),
                    "Please select one of your courses."
                );
            }

            ModelState.Remove(
                nameof(Assignment.Course)
            );

            if (ModelState.IsValid)
            {
                existingAssignment.Title =
                    assignment.Title;

                existingAssignment.Description =
                    assignment.Description;

                existingAssignment.DueDate =
                    DateTime.SpecifyKind(
                        assignment.DueDate,
                        DateTimeKind.Utc
                    );

                existingAssignment.Priority =
                    assignment.Priority;

                existingAssignment.Status =
                    assignment.Status;

                existingAssignment.Completed =
                    IsAssignmentCompleted(
                        assignment.Status
                    );

                existingAssignment.PointsPossible =
                    assignment.PointsPossible;

                existingAssignment.Notes =
                    assignment.Notes;

                existingAssignment.CourseId =
                    assignment.CourseId;

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index)
                );
            }

            var courses =
                await _context.Courses
                    .Where(c =>
                        c.StudentId
                        == student.StudentId)
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();

            ViewData["CourseId"] =
                new SelectList(
                    courses,
                    "CourseId",
                    "CourseName",
                    assignment.CourseId
                );

            return View(assignment);
        }


        // GET: Assignments/Delete/5
        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var assignment =
                await _context.Assignments
                    .Include(a => a.Course)
                    .FirstOrDefaultAsync(a =>
                        a.AssignmentId == id
                        &&
                        a.Course.StudentId
                        == student.StudentId);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }


        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var assignment =
                await _context.Assignments
                    .Include(a => a.Course)
                    .FirstOrDefaultAsync(a =>
                        a.AssignmentId == id
                        &&
                        a.Course.StudentId
                        == student.StudentId);

            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(
                assignment
            );

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}