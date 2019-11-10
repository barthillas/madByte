using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace medBytePresentation.Models.AccountViewModels
{
    public class AuthorizeViewModel
    {
        [Display(Name = "Yetkili")]
        public bool Authorized { get; set; }
    }
}
