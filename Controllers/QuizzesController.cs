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

        // GET: Quizzes
        public async Task<IActionResult> Index()
        {
            var quizzes = _context.Quizzes
                .Include(q => q.Course);

            return View(await quizzes.ToListAsync());
        }

        // GET: Quizzes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }

        // GET: Quizzes/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] =
                new SelectList(_context.Courses, "CourseId", "CourseName");

            return View();
        }

        // POST: Quizzes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] =
                new SelectList(_context.Courses, "CourseId", "CourseName", quiz.CourseId);

            return View(quiz);
        }

        // GET: Quizzes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var quiz = await _context.Quizzes.FindAsync(id);

            if (quiz == null)
                return NotFound();

            ViewData["CourseId"] =
                new SelectList(_context.Courses, "CourseId", "CourseName", quiz.CourseId);

            return View(quiz);
        }

        // POST: Quizzes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quiz quiz)
        {
            if (id != quiz.QuizId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Quizzes.Any(e => e.QuizId == quiz.QuizId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] =
                new SelectList(_context.Courses, "CourseId", "CourseName", quiz.CourseId);

            return View(quiz);
        }

        // GET: Quizzes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }

        // POST: Quizzes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);

            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}