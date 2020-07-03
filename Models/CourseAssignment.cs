using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RCMS_web.Models
{
    public class CourseAssignment
    {
        public int LecturerID { get; set; }
        public int CourseID { get; set; }
        public Lecturer Lecturer { get; set; }
        public Course Course { get; set; }
    }
}