using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Data;
using StudentSuccessDashboard.Models;
using StudentSuccessDashboard.Services;

namespace StudentSuccessDashboard.Controllers
{
    [Authorize]
    public class GradeRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GradeCalculationService _gradeCalculationService;

        public GradeRecordsController(
            ApplicationDbContext context,
            GradeCalculationService gradeCalculationService)
        {
            _context = context;
            _gradeCalculationService = gradeCalculationService;
        }

        // GET: GradeRecords
        public async Task<IActionResult> Index()
        {
            var grades = await _context.GradeRecords
                .Include(g => g.Course)
                .ToListAsync();

            var courseAverages = grades
                .GroupBy(g => g.Course)
                .ToDictionary(
                    group => group.Key.CourseId,
                    group => _gradeCalculationService
                        .CalculateCourseAverage(group));

            ViewBag.CourseAverages = courseAverages;

            var coursesWithGrades = grades
                .Where(g => g.Course != null)
                .GroupBy(g => g.Course)
                .Select(group => new
                {
                    Average = courseAverages[group.Key.CourseId],
                    Credits = group.Key.Credits
                })
                .ToList();

            ViewBag.CumulativeGpa =
                _gradeCalculationService.CalculateCumulativeGpa(
                    coursesWithGrades.Select(c =>
                        (c.Average, c.Credits)));

            return View(grades);
        }

        // GET: GradeRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var grade = await _context.GradeRecords
                .Include(g => g.Course)
                .FirstOrDefaultAsync(
                    g => g.GradeRecordId == id);

            if (grade == null)
                return NotFound();

            return View(grade);
        }

        // GET: GradeRecords/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] =
                new SelectList(
                    _context.Courses,
                    "CourseId",
                    "CourseName");

            return View();
        }

        // POST: GradeRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GradeRecord grade)
        {
            if (ModelState.IsValid)
            {
                var courseExists = await _context.Courses
                    .AnyAsync(c => c.CourseId == grade.CourseId);

                if (!courseExists)
                {
                    ModelState.AddModelError(
                        "CourseId",
                        "The selected course does not exist.");
                }
                else
                {
                    var existingWeight =
                        await _context.GradeRecords
                            .Where(g =>
                                g.CourseId == grade.CourseId)
                            .SumAsync(g => g.Weight);

                    if (existingWeight + grade.Weight > 100)
                    {
                        ModelState.AddModelError(
                            "Weight",
                            $"The total grading weight cannot exceed 100%. " +
                            $"The current total is {existingWeight:0.##}%.");
                    }
                    else
                    {
                        _context.Add(grade);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            ViewData["CourseId"] =
                new SelectList(
                    _context.Courses,
                    "CourseId",
                    "CourseName",
                    grade.CourseId);

            return View(grade);
        }

        // GET: GradeRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var grade = await _context.GradeRecords.FindAsync(id);

            if (grade == null)
                return NotFound();

            ViewData["CourseId"] =
                new SelectList(
                    _context.Courses,
                    "CourseId",
                    "CourseName",
                    grade.CourseId);

            return View(grade);
        }

        // POST: GradeRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            GradeRecord grade)
        {
            if (id != grade.GradeRecordId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var courseExists = await _context.Courses
                    .AnyAsync(c => c.CourseId == grade.CourseId);

                if (!courseExists)
                {
                    ModelState.AddModelError(
                        "CourseId",
                        "The selected course does not exist.");
                }
                else
                {
                    var existingWeight =
                        await _context.GradeRecords
                            .Where(g =>
                                g.CourseId == grade.CourseId &&
                                g.GradeRecordId != grade.GradeRecordId)
                            .SumAsync(g => g.Weight);

                    if (existingWeight + grade.Weight > 100)
                    {
                        ModelState.AddModelError(
                            "Weight",
                            $"The total grading weight cannot exceed 100%. " +
                            $"The current total is {existingWeight:0.##}%.");
                    }
                    else
                    {
                        try
                        {
                            _context.Update(grade);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!_context.GradeRecords.Any(
                                e => e.GradeRecordId ==
                                     grade.GradeRecordId))
                            {
                                return NotFound();
                            }

                            throw;
                        }

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            ViewData["CourseId"] =
                new SelectList(
                    _context.Courses,
                    "CourseId",
                    "CourseName",
                    grade.CourseId);

            return View(grade);
        }

        // GET: GradeRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var grade = await _context.GradeRecords
                .Include(g => g.Course)
                .FirstOrDefaultAsync(
                    g => g.GradeRecordId == id);

            if (grade == null)
                return NotFound();

            return View(grade);
        }

        // POST: GradeRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade =
                await _context.GradeRecords.FindAsync(id);

            if (grade != null)
            {
                _context.GradeRecords.Remove(grade);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}