using FrontToBack.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MaxLength(40)]
        [MinLength(4)]
        public string UserName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
      
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Required]

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }





    }
}
