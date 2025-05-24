using System.ComponentModel.DataAnnotations;

namespace ETForum.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email ili nickname je obavezan")]
        [Display(Name = "Email ili nickname")]

        public string email { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string lozinka { get; set; } = string.Empty;
    }
}
