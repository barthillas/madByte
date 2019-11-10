using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Models.DB
{

    public class ProductsWithCategory
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int ProductCategoryId { get; set; }

        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }


    }
    public class CategoryTree
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CatVar { get; set; }

        public int LeafLevel { get; set; }

        public int CategoryParentId { get; set; }

    }
    public class CategoryParents
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryBreadCrumb { get; set; }

        public string CategoryName { get; set; }

        public int CategoryParentId { get; set; }
       

    }
}
