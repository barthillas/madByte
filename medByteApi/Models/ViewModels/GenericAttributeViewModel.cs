using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Models.ViewModels
{
    public class GenericAttributeViewModel
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public string KeyGroup { get; set; }
        public string Value { get; set; }
        public string Key { get; set; }
    }
}
