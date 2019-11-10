using System;
using System.Collections.Generic;

namespace medByteApi.Models.DB
{
    public partial class GenericAttribute
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public string KeyGroup { get; set; }
        public string Value { get; set; }
        public string Key { get; set; }
    }
}
