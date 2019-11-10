using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryParentId { get; set; }
        public string CatVar { get; set; }
        public int Level { get; set; }

    }
}

