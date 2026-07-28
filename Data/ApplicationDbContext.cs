using Microsoft.EntityFrameworkCore;
using StudentSuccessDashboard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace StudentSuccessDashboard.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        // NEW
        public DbSet<Semester> Semesters { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<StudySession> StudySessions { get; set; }

        public DbSet<GradeRecord> GradeRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student -> Semester (1 to Many)
            modelBuilder.Entity<Semester>()
                .HasOne(s => s.Student)
                .WithMany(s => s.Semesters)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Semester -> Course (1 to Many)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Semester)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student -> Course (existing relationship)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Student)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}