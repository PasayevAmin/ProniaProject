using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class UpdateColorVM
    {
        [Required(ErrorMessage = "Name must be entered")]
        [MaxLength(25, ErrorMessage = "Name cannot be longer than 25")]
        public string Name { get; set; }
    }
}
