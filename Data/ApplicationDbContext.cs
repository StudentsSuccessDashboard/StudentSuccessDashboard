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

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<StudySession> StudySessions { get; set; }

        public DbSet<GradeRecord> GradeRecords { get; set; }

    }
}