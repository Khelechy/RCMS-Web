using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RCMS_web.Data;
using RCMS_web.Models;
using RCMS_web.Models.SchoolViewModels;

namespace RCMS_web.Controllers
{
    public class LecturersController : Controller
    {
        private readonly SchoolContext _context;

        public LecturersController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Lecturers
        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            var viewModel = new LecturerIndexData();
            viewModel.Lecturers = await _context.Lecturers
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Student)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();
            
            if (id != null)
            {
                ViewData["LecturerID"] = id.Value;
                Lecturer lecturer = viewModel.Lecturers.Where(
                    i => i.ID == id.Value).Single();
                viewModel.Courses = lecturer.CourseAssignments.Select(s => s.Course);
            }

            if (courseID != null)
            {
                ViewData["CourseID"] = courseID.Value;
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }


        // GET: Lecturers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (lecturer == null)
            {
                return NotFound();
            }

            return View(lecturer);
        }

        // GET: Lecturers/Create
        public IActionResult Create()
        {
            var lecturer = new Lecturer();
            lecturer.CourseAssignments = new List<CourseAssignment>();
            PopulateAssignedCourseData(lecturer);
            return View();
        }

        // POST: Lecturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstMidName,Email,LastName,OfficeAssignment")] Lecturer lecturer, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                lecturer.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { LecturerID = lecturer.ID, CourseID = int.Parse(course) };
                    lecturer.CourseAssignments.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(lecturer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedCourseData(lecturer);
            return View(lecturer);
        }

        // GET: Lecturers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lecturer = await _context.Lecturers
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (lecturer == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(lecturer);
            return View(lecturer);
        }

        private void PopulateAssignedCourseData(Lecturer lecturer)
        {
            var allCourses = _context.Courses;
            var lecturerCourses = new HashSet<int>(lecturer.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = lecturerCourses.Contains(course.CourseID)
                });
            }
            ViewData["Courses"] = viewModel;
        }

        // POST: Lecturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerToUpdate = await _context.Lecturers
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (await TryUpdateModelAsync<Lecturer>(
                lecturerToUpdate,
                "",
                i => i.FirstMidName, i => i.LastName, i => i.Email, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(lecturerToUpdate.OfficeAssignment?.Location))
                {
                    lecturerToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedCourses, lecturerToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateInstructorCourses(selectedCourses, lecturerToUpdate);
            PopulateAssignedCourseData(lecturerToUpdate);
            return View(lecturerToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Lecturer lecturerToUpdate)
        {
            if (selectedCourses == null)
            {
                lecturerToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var lecturerCourses = new HashSet<int>
                (lecturerToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!lecturerCourses.Contains(course.CourseID))
                    {
                        lecturerToUpdate.CourseAssignments.Add(new CourseAssignment { LecturerID = lecturerToUpdate.ID, CourseID = course.CourseID });
                    }
                }
                else
                {

                    if (lecturerCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove = lecturerToUpdate.CourseAssignments.FirstOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }


        // GET: Lecturers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (lecturer == null)
            {
                return NotFound();
            }

            return View(lecturer);
        }

        // POST: Lecturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Lecturer lecturer = await _context.Lecturers
                .Include(i => i.CourseAssignments)
                .SingleAsync(i => i.ID == id);

            var departments = await _context.Departments
                .Where(d => d.LecturerID == id)
                .ToListAsync();
            departments.ForEach(d => d.LecturerID = null);
            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LecturerExists(int id)
        {
            return _context.Lecturers.Any(e => e.ID == id);
        }
    }
}
