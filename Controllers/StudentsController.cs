using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RCMS_web.Data;
using RCMS_web.Models;
using MimeKit;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using RCMS_web.Services;
using Microsoft.AspNetCore.Mvc.Rendering;





namespace RCMS_web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private IViewRenderer _renderer;

        public StudentsController(SchoolContext context, IViewRenderer renderer)
        {
            _context = context;
            _renderer = renderer;
        }

        // GET: Students
     public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var students = from s in _context.Students.Include(d => d.Department)
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.ToLower().Contains(searchString.ToLower())
                                    || s.FirstMidName.ToLower().Contains(searchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.MatricNo);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.MatricNo);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            
            int pageSize = 5;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .ThenInclude(d => d.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/SendMail/5
        public async Task<IActionResult> SendMail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .ThenInclude(d => d.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            Dictionary<string, string> dicGrades = new Dictionary<string, string>();
            foreach(var item in student.Enrollments){
                dicGrades.Add(item.Course.Title, item.Grade.ToString());
            }

            var trs = dicGrades.Select(a => String.Format("<tr><td>{0}</td><td>{1}</td></tr>", a.Key, a.Value));

            var tableContents = String.Concat(trs);

            var table = "<table>" + tableContents + "</table>";
            

            string htmlBody = "<h3> Name: " + student.FullName + "</h3><h3> Matric No: " + student.MatricNo + "</h3><h3> Email: " + student.Email + "</h3><h3> Phone Number: " + student.PhoneNumber + "</h3><h3> Grades: " + table + "</h3>";
            SendTheEmail(student, htmlBody);

            return View(student);
        }


        // GET: Students/SendSms/5
        public async Task<IActionResult> SendSms(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            Dictionary<string, string> dicGrades = new Dictionary<string, string>();
            foreach(var item in student.Enrollments){
                dicGrades.Add(item.Course.Title, item.Grade.ToString());
            }

            var trs = dicGrades.Select(a => String.Format(" {0} = {1} , ", a.Key, a.Value));

            var tableContents = String.Concat(trs);

            var table = tableContents;

            var studentPhoneNo = student.PhoneNumber;
            

            string smsBody = " Name: " + student.FullName + " Matric No: " + student.MatricNo + " Email: " + student.Email + " Phone Number: " + student.PhoneNumber + " Grades: " + table;
            SendTheSms(student, smsBody, studentPhoneNo).Wait();

            return View(student);
        }

        public async void SendTheEmail(Student student, string ScrappedView, bool sendAsync = true){
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("UNN Result Admin", 
            "mconyekwerekelechi@gmail.com");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(student.FirstMidName, 
            student.Email);
            message.To.Add(to);

            message.Subject = student.FirstMidName + " your result is ready";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = ScrappedView;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("mconyekwerekelechi@gmail.com", "macmillian3"); // If using GMail this requires turning on LessSecureApps : https://myaccount.google.com/lesssecureapps
                if (sendAsync)
                {
                    await client.SendAsync(message);
                }
                else
                {
                    client.Send(message);
                }
                client.Disconnect(true);
            }


        }

        public async Task SendTheSms(Student student, string smsBody, string studentPhoneNo){
           // Find your Account Sid and Token at twilio.com/console
            const string accountSid = "AC8bf3b6275b87918384cd7e3d3a39da1c";
            const string authToken = "cf6354b6d7f55d9b626f2ec0ddc9b4c9";

            TwilioClient.Init(accountSid, authToken);

            var message = await MessageResource.CreateAsync(
                body: smsBody,
                from: new Twilio.Types.PhoneNumber("+12136994794"),
                to: new Twilio.Types.PhoneNumber(studentPhoneNo)
            );

            Console.WriteLine(message.Sid);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,MatricNo,Email,PhoneNumber,DepartmentID")] Student student)
        {
            try{
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator.");
                }
            PopulateDepartmentsDropDownList(student.DepartmentID);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.MatricNo, s => s.Email, s => s.PhoneNumber, s => s.DepartmentID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                orderby d.Name
                select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }


            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try{
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
                }
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }


        
    }

    // public static class Extension{
    //     public static string PathToRazorPageFolder(this HttpRequest request)
    //     {
    //         if (request != null) {
    //             var requestPath = request.Path.ToString();
    //             var returnPathToFolder = request.Scheme + "://" + request.Host + requestPath.Substring(0, requestPath.LastIndexOf("/")); ;
    //             return returnPathToFolder;
    //         } else
    //         {
    //             return "HttpRequest was null";
    //         }
    //     }
    // }

}
