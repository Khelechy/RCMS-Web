﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RCMS_web.Data;

namespace RCMS_web.Migrations
{
    [DbContext(typeof(SchoolContext))]
    [Migration("20200702202708_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("RCMS_web.Models.Course", b =>
                {
                    b.Property<int>("CourseID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Credits")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DepartmentID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("CourseID");

                    b.HasIndex("DepartmentID");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("RCMS_web.Models.CourseAssignment", b =>
                {
                    b.Property<int>("CourseID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LecturerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("CourseID", "LecturerID");

                    b.HasIndex("LecturerID");

                    b.ToTable("CourseAssignment");
                });

            modelBuilder.Entity("RCMS_web.Models.Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Budget")
                        .HasColumnType("money");

                    b.Property<int?>("LecturerID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("DepartmentID");

                    b.HasIndex("LecturerID");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("RCMS_web.Models.Enrollment", b =>
                {
                    b.Property<int>("EnrollmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CourseID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StudentID")
                        .HasColumnType("INTEGER");

                    b.HasKey("EnrollmentID");

                    b.HasIndex("CourseID");

                    b.HasIndex("StudentID");

                    b.ToTable("Enrollment");
                });

            modelBuilder.Entity("RCMS_web.Models.Lecturer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstMidName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(20);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(20);

                    b.HasKey("ID");

                    b.ToTable("Lecturer");
                });

            modelBuilder.Entity("RCMS_web.Models.OfficeAssignment", b =>
                {
                    b.Property<int>("LecturerID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT")
                        .HasMaxLength(20);

                    b.HasKey("LecturerID");

                    b.ToTable("OfficeAssignment");
                });

            modelBuilder.Entity("RCMS_web.Models.Student", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstMidName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("MatricNo")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("RCMS_web.Models.Course", b =>
                {
                    b.HasOne("RCMS_web.Models.Department", "Department")
                        .WithMany("Courses")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RCMS_web.Models.CourseAssignment", b =>
                {
                    b.HasOne("RCMS_web.Models.Course", "Course")
                        .WithMany("CourseAssignments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RCMS_web.Models.Lecturer", "Lecturer")
                        .WithMany("CourseAssignments")
                        .HasForeignKey("LecturerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RCMS_web.Models.Department", b =>
                {
                    b.HasOne("RCMS_web.Models.Lecturer", "Administrator")
                        .WithMany()
                        .HasForeignKey("LecturerID");
                });

            modelBuilder.Entity("RCMS_web.Models.Enrollment", b =>
                {
                    b.HasOne("RCMS_web.Models.Course", "Course")
                        .WithMany("Enrollments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RCMS_web.Models.Student", "Student")
                        .WithMany("Enrollments")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RCMS_web.Models.OfficeAssignment", b =>
                {
                    b.HasOne("RCMS_web.Models.Lecturer", "Lecturer")
                        .WithOne("OfficeAssignment")
                        .HasForeignKey("RCMS_web.Models.OfficeAssignment", "LecturerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
