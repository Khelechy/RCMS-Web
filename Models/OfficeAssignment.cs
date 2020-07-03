using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RCMS_web.Models
{
    public class OfficeAssignment
    {
        [Key]
        public int LecturerID { get; set; }
        [StringLength(20)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public Lecturer Lecturer { get; set; }
    }
}