using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RCMS_web.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        public int? LecturerID { get; set; }


        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Lecturer Administrator { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}