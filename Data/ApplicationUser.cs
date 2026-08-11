using Microsoft.AspNetCore.Identity;
using StudentSuccessDashboard.Models;

namespace StudentSuccessDashboard.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public ICollection<StudySession> StudySessions { get; set; }
            = new List<StudySession>();
    }
}