using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RCMS_web.Data;
using RCMS_web.Models;

namespace RCMS_web.Controllers
{
	public class AccountController : Controller
	{
		private readonly SchoolContext _context;
		public AccountController(SchoolContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
				try
				{
                    //var admin = await _context.Admins
                    // .SingleOrDefaultAsync(m => m.Email == model.Email && m.Password == model.Password);
                    var admin = await  _context.Admins
                        .Where(m => m.Email == model.Email && m.Password == model.Password).SingleOrDefaultAsync();
                    if (admin == null)
                    {
                        ModelState.AddModelError("Password", "Invalid login attempt.");
                        return View("Index");
                    }
                    HttpContext.Session.SetString("userId", admin.Name);

				}
				catch (Exception   e)
				{
                    var x = e;
				}
            }
            else
            {
                return View("Index");
            }
            return RedirectToAction("Index", "Student");
        }

        //[HttpPost]
        //public async Task<ActionResult> Register(RegistrationViewModel model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        Admin admin = new Admin
        //        {
        //            Name = model.Name,
        //            Email = model.Email,
        //            Password = model.Password
        //        };
        //       // _context.Add(admin);
        //        _context.Admins.Add(admin);
        //        await _context.SaveChangesAsync();

        //    }
        //    else
        //    {
        //        return View("Registration");
        //    }
        //    return RedirectToAction("Index", "Account");
        //}

        // registration Page load
        public IActionResult Registration()
        {
            ViewData["Message"] = "Registration Page";

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        public void ValidationMessage(string key, string alert, string value)
        {
            try
            {
                TempData.Remove(key);
                TempData.Add(key, value);
                TempData.Add("alertType", alert);
            }
            catch
            {
                Debug.WriteLine("TempDataMessage Error");
            }

        }

    }
}
