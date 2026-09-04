using System.Security.Claims;
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


        // GET: GradeRecords
        public async Task<IActionResult> Index()
        {
            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var grades =
                await _context.GradeRecords
                    .Include(g => g.Course)
                    .Where(g =>
                        g.Course.StudentId
                        == student.StudentId)
                    .OrderBy(g => g.Course.CourseName)
                    .ThenBy(g => g.Category)
                    .ToListAsync();

            var courseAverages =
                grades
                    .Where(g => g.Course != null)
                    .GroupBy(g => g.Course)
                    .ToDictionary(
                        group =>
                            group.Key.CourseId,
                        group =>
                            _gradeCalculationService
                                .CalculateCourseAverage(
                                    group
                                )
                    );

            ViewBag.CourseAverages =
                courseAverages;

            var coursesWithGrades =
                grades
                    .Where(g => g.Course != null)
                    .GroupBy(g => g.Course)
                    .Select(group => new
                    {
                        Average =
                            courseAverages[
                                group.Key.CourseId
                            ],

                        Credits =
                            group.Key.Credits
                    })
                    .ToList();

            ViewBag.CumulativeGpa =
                _gradeCalculationService
                    .CalculateCumulativeGpa(
                        coursesWithGrades
                            .Select(c =>
                                (
                                    c.Average,
                                    c.Credits
                                )
                            )
                    );

            return View(grades);
        }


        // GET: GradeRecords/Details/5
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

            var grade =
                await _context.GradeRecords
                    .Include(g => g.Course)
                    .FirstOrDefaultAsync(g =>
                        g.GradeRecordId == id
                        &&
                        g.Course.StudentId
                        == student.StudentId);

            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }


        // GET: GradeRecords/Create
        public async Task<IActionResult> Create()
        {
            var courseList =
                await GetCourseSelectListAsync();

            if (courseList == null)
            {
                return Unauthorized();
            }

            ViewData["CourseId"] =
                courseList;

            return View();
        }


        // POST: GradeRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            GradeRecord grade)
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
                        == grade.CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(GradeRecord.CourseId),
                    "Please select one of your courses."
                );
            }

            ModelState.Remove(
                nameof(GradeRecord.Course)
            );

            if (ModelState.IsValid)
            {
                var existingWeight =
                    await _context.GradeRecords
                        .Where(g =>
                            g.CourseId
                            == grade.CourseId)
                        .SumAsync(g => g.Weight);

                if (existingWeight
                    + grade.Weight > 100)
                {
                    ModelState.AddModelError(
                        nameof(GradeRecord.Weight),
                        "The total grading weight " +
                        "cannot exceed 100%. " +
                        $"The current total is " +
                        $"{existingWeight:0.##}%."
                    );
                }
                else
                {
                    _context.GradeRecords.Add(
                        grade
                    );

                    await _context.SaveChangesAsync();

                    return RedirectToAction(
                        nameof(Index)
                    );
                }
            }

            ViewData["CourseId"] =
                await GetCourseSelectListAsync(
                    grade.CourseId
                );

            return View(grade);
        }


        // GET: GradeRecords/Edit/5
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

            var grade =
                await _context.GradeRecords
                    .Include(g => g.Course)
                    .FirstOrDefaultAsync(g =>
                        g.GradeRecordId == id
                        &&
                        g.Course.StudentId
                        == student.StudentId);

            if (grade == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] =
                await GetCourseSelectListAsync(
                    grade.CourseId
                );

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
            {
                return NotFound();
            }

            var student =
                await GetCurrentStudentAsync();

            if (student == null)
            {
                return Unauthorized();
            }

            var existingGrade =
                await _context.GradeRecords
                    .Include(g => g.Course)
                    .FirstOrDefaultAsync(g =>
                        g.GradeRecordId == id
                        &&
                        g.Course.StudentId
                        == student.StudentId);

            if (existingGrade == null)
            {
                return NotFound();
            }

            var courseBelongsToStudent =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseId
                        == grade.CourseId
                        &&
                        c.StudentId
                        == student.StudentId);

            if (!courseBelongsToStudent)
            {
                ModelState.AddModelError(
                    nameof(GradeRecord.CourseId),
                    "Please select one of your courses."
                );
            }

            ModelState.Remove(
                nameof(GradeRecord.Course)
            );

            if (ModelState.IsValid)
            {
                var existingWeight =
                    await _context.GradeRecords
                        .Where(g =>
                            g.CourseId
                            == grade.CourseId
                            &&
                            g.GradeRecordId
                            != grade.GradeRecordId)
                        .SumAsync(g => g.Weight);

                if (existingWeight
                    + grade.Weight > 100)
                {
                    ModelState.AddModelError(
                        nameof(GradeRecord.Weight),
                        "The total grading weight " +
                        "cannot exceed 100%. " +
                        $"The current total is " +
                        $"{existingWeight:0.##}%."
                    );
                }
                else
                {
                    existingGrade.Category =
                        grade.Category;

                    existingGrade.Score =
                        grade.Score;

                    existingGrade.Weight =
                        grade.Weight;

                    existingGrade.CourseId =
                        grade.CourseId;

                    await _context.SaveChangesAsync();

                    return RedirectToAction(
                        nameof(Index)
                    );
                }
            }

            ViewData["CourseId"] =
                await GetCourseSelectListAsync(
                    grade.CourseId
                );

            return View(grade);
        }


        // GET: GradeRecords/Delete/5
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

            var grade =
                await _context.GradeRecords
                    .Include(g => g.Course)
                    .FirstOrDefaultAsync(g =>
                        g.GradeRecordId == id
                        &&
                        g.Course.StudentId
                        == student.StudentId);

            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }


        // POST: GradeRecords/Delete/5
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

            var grade =
                await _context.GradeRecords
                    .Include(g => g.Course)
                    .FirstOrDefaultAsync(g =>
                        g.GradeRecordId == id
                        &&
                        g.Course.StudentId
                        == student.StudentId);

            if (grade == null)
            {
                return NotFound();
            }

            _context.GradeRecords.Remove(
                grade
            );

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}