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

        public StudySessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySessions = await _context.StudySessions
                .Include(s => s.Course)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();

            return View(studySessions);
        }

        public IActionResult Create()
        {
            ViewBag.CourseId = new SelectList(
                _context.Courses,
                "CourseId",
                "CourseName"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StudySession studySession)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            studySession.UserId = userId;

            studySession.SessionDate = DateTime.SpecifyKind(
                studySession.SessionDate,
                DateTimeKind.Utc
            );

            ModelState.Remove(nameof(StudySession.UserId));
            ModelState.Remove(nameof(StudySession.User));
            ModelState.Remove(nameof(StudySession.Course));

            if (ModelState.IsValid)
            {
                _context.StudySessions.Add(studySession);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseId = new SelectList(
                _context.Courses,
                "CourseId",
                "CourseName",
                studySession.CourseId
            );

            return View(studySession);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySession =
                await _context.StudySessions
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id &&
                        s.UserId == userId);

            if (studySession == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = new SelectList(
                _context.Courses,
                "CourseId",
                "CourseName",
                studySession.CourseId
            );

            return View(studySession);
        }

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
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var existingSession =
                await _context.StudySessions
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id &&
                        s.UserId == userId);

            if (existingSession == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(StudySession.UserId));
            ModelState.Remove(nameof(StudySession.User));
            ModelState.Remove(nameof(StudySession.Course));

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

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseId = new SelectList(
                _context.Courses,
                "CourseId",
                "CourseName",
                studySession.CourseId
            );

            return View(studySession);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySession =
                await _context.StudySessions
                    .Include(s => s.Course)
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id &&
                        s.UserId == userId);

            if (studySession == null)
            {
                return NotFound();
            }

            return View(studySession);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var studySession =
                await _context.StudySessions
                    .FirstOrDefaultAsync(s =>
                        s.StudySessionId == id &&
                        s.UserId == userId);

            if (studySession == null)
            {
                return NotFound();
            }

            _context.StudySessions.Remove(studySession);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Timer()
        {
            ViewBag.CourseId = new SelectList(
                _context.Courses,
                "CourseId",
                "CourseName"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTimerSession(
            int CourseId,
            string Topic,
            string? Notes,
            int DurationMinutes)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (CourseId <= 0 ||
                string.IsNullOrWhiteSpace(Topic) ||
                DurationMinutes < 1)
            {
                return BadRequest();
            }

            var studySession = new StudySession
            {
                CourseId = CourseId,
                Topic = Topic,
                Notes = Notes,
                DurationMinutes = DurationMinutes,

                // PostgreSQL requires UTC
                SessionDate = DateTime.UtcNow,

                UserId = userId
            };

            _context.StudySessions.Add(studySession);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}