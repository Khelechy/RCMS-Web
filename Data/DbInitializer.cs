using RCMS_web.Models;
using System;
using System.Linq;

namespace RCMS_web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            //context.Database.EnsureCreated();

            // Look for any students.
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }

            var students = new Student[]
            {
                new Student{FirstMidName="Carson",LastName="Alexander",MatricNo="2017/24654"},
                new Student{FirstMidName="John",LastName="Mont",MatricNo="2017/24554"},
                new Student{FirstMidName="Queen",LastName="Oliver",MatricNo="2017/26654"},
                new Student{FirstMidName="Killian",LastName="Yandex",MatricNo="2017/27654"},
                new Student{FirstMidName="Megan",LastName="Joseph",MatricNo="2017/24764"},
                new Student{FirstMidName="Carl",LastName="Allison",MatricNo="2017/24984"},
                new Student{FirstMidName="Nino",LastName="Olivetto",MatricNo="2017/245333"}
            };
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var lecturers = new Lecturer[]
            {
                new Lecturer { FirstMidName = "James",     LastName = "Abercrombie",
                    Email="james.kele@gmail.com" },
                new Lecturer { FirstMidName = "Anthony",     LastName = "Abemane",
                    Email="anthony.kele@gmail.com" },
                new Lecturer { FirstMidName = "King",     LastName = "Berkely",
                    Email="king.kele@gmail.com" },
                new Lecturer { FirstMidName = "Price",     LastName = "John",
                    Email="prince.kele@gmail.com" },
                new Lecturer { FirstMidName = "John",     LastName = "Crombie",
                    Email="john.kele@gmail.com" }
            };

            foreach (Lecturer i in lecturers)
            {
                context.Lecturers.Add(i);
            }
            context.SaveChanges();

            var departments = new Department[]
            {
                new Department { Name = "Computer Science",     Budget = 350000,
                    LecturerID = lecturers.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000,
                    LecturerID  = lecturers.Single( i => i.LastName == "Berkely").ID },
                new Department { Name = "Statistics", Budget = 350000,
                    LecturerID  = lecturers.Single( i => i.LastName == "John").ID },
                new Department { Name = "Economics",   Budget = 100000,
                    LecturerID  = lecturers.Single( i => i.LastName == "Crombie").ID }
            };

            foreach (Department d in departments)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course{CourseID=1050,Title="Cos432",Credits=3, DepartmentID = departments.Single( s => s.Name == "Computer Science").DepartmentID},
                new Course{CourseID=4022,Title="Sta451",Credits=3, DepartmentID = departments.Single( s => s.Name == "Statistics").DepartmentID},
                new Course{CourseID=4041,Title="Mth402",Credits=3, DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID},
                new Course{CourseID=1045,Title="Mth433",Credits=4, DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID},
                new Course{CourseID=3141,Title="Eco443",Credits=4, DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID},
                new Course{CourseID=2021,Title="Cos452",Credits=3, DepartmentID = departments.Single( s => s.Name == "Computer Science").DepartmentID},
                new Course{CourseID=2042,Title="Cos401",Credits=4, DepartmentID = departments.Single( s => s.Name == "Computer Science").DepartmentID}
            };
            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment {
                    LecturerID = lecturers.Single( i => i.LastName == "Crombie").ID,
                    Location = "Smith 17" },
                new OfficeAssignment {
                    LecturerID = lecturers.Single( i => i.LastName == "Abercrombie").ID,
                    Location = "Abuja Building" },
                new OfficeAssignment {
                    LecturerID = lecturers.Single( i => i.LastName == "John").ID,
                    Location = "Thompson 304" },
            };

            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o);
            }
            context.SaveChanges();

            var courseLecturers = new CourseAssignment[]
            {
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Cos432" ).CourseID,
                    LecturerID = lecturers.Single( i => i.LastName == "Crombie").ID
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Cos401" ).CourseID,
                    LecturerID = lecturers.Single( i => i.LastName == "John").ID
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Eco443" ).CourseID,
                    LecturerID = lecturers.Single( i => i.LastName == "Berkely").ID
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Sta451" ).CourseID,
                    LecturerID = lecturers.Single( i => i.LastName == "Abercrombie").ID
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Mth433" ).CourseID,
                    LecturerID = lecturers.Single( i => i.LastName == "Crombie").ID
                    },
            };

            foreach (CourseAssignment ci in courseLecturers)
            {
                context.CourseAssignments.Add(ci);
            }
            context.SaveChanges();

            // var enrollments = new Enrollment[]
            // {
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Alexander").ID,
            //         CourseID = courses.Single(c => c.Title == "Cos432" ).CourseID,
            //         Grade = Grade.A
            //     },
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Alexander").ID,
            //         CourseID = courses.Single(c => c.Title == "Eco443" ).CourseID,
            //         Grade = Grade.C
            //     },
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Joseph").ID,
            //         CourseID = courses.Single(c => c.Title == "Cos401" ).CourseID,
            //         Grade = Grade.B
            //     },
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Mont").ID,
            //         CourseID = courses.Single(c => c.Title == "Mth402" ).CourseID,
            //         Grade = Grade.B
            //     },
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Oliver").ID,
            //         CourseID = courses.Single(c => c.Title == "Sta451").CourseID,
            //         Grade = Grade.B
            //     },
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Oliver").ID,
            //         CourseID = courses.Single(c => c.Title == "Mth433").CourseID,
            //         Grade = Grade.B
            //     },
            //     new Enrollment {
            //         StudentID = students.Single(s => s.LastName == "Yandex").ID,
            //         CourseID = courses.Single(c => c.Title == "Eco443").CourseID,
            //         Grade = Grade.B
            //     }
            // };

            var enrollments = new Enrollment[]
            {
            new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
            new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
            new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
            new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
            new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
            new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
            new Enrollment{StudentID=3,CourseID=1050},
            new Enrollment{StudentID=4,CourseID=1050},
            new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
            new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
            new Enrollment{StudentID=3,CourseID=1045},
            new Enrollment{StudentID=3,CourseID=3141,Grade=Grade.A},
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                            s.Student.ID == e.StudentID &&
                            s.Course.CourseID == e.CourseID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
        }
    }
}
