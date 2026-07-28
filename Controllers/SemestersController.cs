using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            var semesters = await _context.Semesters
                .Include(s => s.Student)
                .Include(s => s.Courses)
                .ToListAsync();

            return View(semesters);
        }


        // GET: Semesters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var semester = await _context.Semesters
                .Include(s => s.Student)
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.SemesterId == id);

            if (semester == null)
                return NotFound();

            return View(semester);
        }


        // GET: Semesters/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "Email"
            );

            return View();
        }


        // POST: Semesters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semester semester)
        {
            if (ModelState.IsValid)
            {
                _context.Add(semester);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "Email",
                semester.StudentId
            );

            return View(semester);
        }


        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var semester = await _context.Semesters.FindAsync(id);

            if (semester == null)
                return NotFound();


            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "Email",
                semester.StudentId
            );


            return View(semester);
        }


        // POST: Semesters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Semester semester)
        {
            if (id != semester.SemesterId)
                return NotFound();


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(semester);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SemesterExists(semester.SemesterId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }


            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "Email",
                semester.StudentId
            );


            return View(semester);
        }


        // GET: Semesters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();


            var semester = await _context.Semesters
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.SemesterId == id);


            if (semester == null)
                return NotFound();


            return View(semester);
        }


        // POST: Semesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);


            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }


        private bool SemesterExists(int id)
        {
            return _context.Semesters.Any(e => e.SemesterId == id);
        }
    }
}