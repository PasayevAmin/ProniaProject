﻿using FrontToBack.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> ColorIds { get; set; }
        public List<int> SizeIds { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Color>? Colors { get; set; }

       
    }
}
