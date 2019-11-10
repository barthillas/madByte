using System;
using System.Collections.Generic;

namespace medByteApi.Models.DB
{
    public partial class Categories
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryParentId { get; set; }
    }
}
