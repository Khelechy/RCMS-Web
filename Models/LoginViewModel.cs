using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RCMS_web.Models
{
	public class LoginViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		[StringLength(10, MinimumLength = 6)]
		public string Password { get; set; }
	}
}
