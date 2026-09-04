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
    public class StudySessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudySessionsController(
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

        private async Task<SelectList?> GetCourseSelectListAsync(
            int? selectedCourseId = null)
        {
            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return null;
            }

            var courses =
                await _context.Courses
                    .Where(c =>
                        c.StudentId
                        == student.StudentId)
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();

            return new SelectList(
                courses,
                "CourseId",
                "CourseName",
                selectedCourseId
            );
        }


        // GET: StudySessions
        public async Task<IActionResult> Index()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySessions =
                await _context.StudySessions
                    .Include(s => s.Course)
                    .Where(s =>
                        s.UserId == userId)
                    .OrderByDescending(
                        s => s.SessionDate)
                    .ToListAsync();

            return View(studySessions);
        }


        // GET: StudySessions/Create
        public async Task<IActionResult> Create()
        {
            var courseList =
                await GetCourseSelectListAsync();

            if (courseList == null)
            {
                return Unauthorized();
            }

            ViewBag.CourseId = courseList;

            return View();
        }


        // POST: StudySessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StudySession studySession)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            var student =
                await GetCurrentStudentAsync();

            if (userId == null ||
                student == null)
            {
                return Unauthorized();
            }

            var courseBelongsToStudent =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseId
                        == studySession.CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(StudySession.CourseId),
                    "Please select one of your courses."
                );
            }

            studySession.UserId = userId;

            studySession.SessionDate =
                DateTime.SpecifyKind(
                    studySession.SessionDate,
                    DateTimeKind.Utc
                );

            ModelState.Remove(
                nameof(StudySession.UserId)
            );

            ModelState.Remove(
                nameof(StudySession.User)
            );

            ModelState.Remove(
                nameof(StudySession.Course)
            );

            if (ModelState.IsValid)
            {
                _context.StudySessions.Add(
                    studySession
                );

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index)
                );
            }

            ViewBag.CourseId =
                await GetCourseSelectListAsync(
                    studySession.CourseId
                );

            return View(studySession);
        }


        // GET: StudySessions/Edit/5
        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySession =
                await _context.StudySessions
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id
                        &&
                        s.UserId == userId);

            if (studySession == null)
            {
                return NotFound();
            }

            ViewBag.CourseId =
                await GetCourseSelectListAsync(
                    studySession.CourseId
                );

            return View(studySession);
        }


        // POST: StudySessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            StudySession studySession)
        {
            if (id != studySession.StudySessionId)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            var student =
                await GetCurrentStudentAsync();

            if (userId == null ||
                student == null)
            {
                return Unauthorized();
            }

            var existingSession =
                await _context.StudySessions
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id
                        &&
                        s.UserId == userId);

            if (existingSession == null)
            {
                return NotFound();
            }

            var courseBelongsToStudent =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseId
                        == studySession.CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(StudySession.CourseId),
                    "Please select one of your courses."
                );
            }

            ModelState.Remove(
                nameof(StudySession.UserId)
            );

            ModelState.Remove(
                nameof(StudySession.User)
            );

            ModelState.Remove(
                nameof(StudySession.Course)
            );

            if (ModelState.IsValid)
            {
                existingSession.CourseId =
                    studySession.CourseId;

                existingSession.Topic =
                    studySession.Topic;

                existingSession.SessionDate =
                    DateTime.SpecifyKind(
                        studySession.SessionDate,
                        DateTimeKind.Utc
                    );

                existingSession.DurationMinutes =
                    studySession.DurationMinutes;

                existingSession.Notes =
                    studySession.Notes;

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index)
                );
            }

            ViewBag.CourseId =
                await GetCourseSelectListAsync(
                    studySession.CourseId
                );

            return View(studySession);
        }


        // GET: StudySessions/Delete/5
        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySession =
                await _context.StudySessions
                    .Include(s => s.Course)
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id
                        &&
                        s.UserId == userId);

            if (studySession == null)
            {
                return NotFound();
            }

            return View(studySession);
        }


        // POST: StudySessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySession =
                await _context.StudySessions
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id
                        &&
                        s.UserId == userId);

            if (studySession == null)
            {
                return NotFound();
            }

            _context.StudySessions.Remove(
                studySession
            );

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index)
            );
        }


        // GET: StudySessions/Timer
        public async Task<IActionResult> Timer()
        {
            var courseList =
                await GetCourseSelectListAsync();

            if (courseList == null)
            {
                return Unauthorized();
            }

            ViewBag.CourseId = courseList;

            return View();
        }


        // POST: StudySessions/SaveTimerSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            SaveTimerSession(
                int CourseId,
                string Topic,
                string? Notes,
                int DurationMinutes)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            var student =
                await GetCurrentStudentAsync();

            if (userId == null ||
                student == null)
            {
                return Unauthorized();
            }

            if (CourseId <= 0 ||
                string.IsNullOrWhiteSpace(Topic) ||
                DurationMinutes < 1)
            {
                return BadRequest();
            }

            var courseBelongsToStudent =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseId == CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                return BadRequest();
            }

            var studySession =
                new StudySession
                {
                    CourseId = CourseId,

                    Topic = Topic.Trim(),

                    Notes =
                        string.IsNullOrWhiteSpace(Notes)
                            ? null
                            : Notes.Trim(),

                    DurationMinutes =
                        DurationMinutes,

                    SessionDate =
                        DateTime.UtcNow,

                    UserId = userId
                };

            _context.StudySessions.Add(
                studySession
            );

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}