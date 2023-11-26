using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class CreateSlideVM
    {

        [Required]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
    }
}
