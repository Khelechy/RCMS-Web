using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RCMS_web.Models.SchoolViewModels
{
    public class LecturerIndexData
    {
        public IEnumerable<Lecturer> Lecturers { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}