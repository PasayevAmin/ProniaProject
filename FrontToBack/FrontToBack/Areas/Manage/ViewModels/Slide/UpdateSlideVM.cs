using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class UpdateSlideVM
    {

        
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        
        public IFormFile? Photo { get; set; }
    }
}
