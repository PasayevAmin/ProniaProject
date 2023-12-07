using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class LoginVM
    {
        [Required]
        [MinLength(4)]
        [MaxLength(255)]
        public string UserNameOrEmail { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
    }
}
