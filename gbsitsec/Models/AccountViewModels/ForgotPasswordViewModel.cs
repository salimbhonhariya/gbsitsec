using System.ComponentModel.DataAnnotations;

namespace gbsitsec.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}