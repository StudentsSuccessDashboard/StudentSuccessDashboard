using System.ComponentModel.DataAnnotations;

namespace StudentSuccessDashboard.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string? LastName { get; set; }

        public string? StatusMessage { get; set; }
    }
}