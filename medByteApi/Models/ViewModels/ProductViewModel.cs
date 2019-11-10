using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Models.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
    }
}

