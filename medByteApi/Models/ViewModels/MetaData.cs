using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Models.ViewModels
{
    public class MetaData
    {
        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
