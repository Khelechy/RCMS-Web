using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RCMS_web.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        [Column("FirstName")]
        public string FirstMidName { get; set; }

        public string Email {get; set; }

        public string PhoneNumber {get; set; }

        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        public Department Department { get; set; }


        public string MatricNo { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }
        
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}