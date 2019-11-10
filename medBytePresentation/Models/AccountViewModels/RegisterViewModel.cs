using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace medBytePresentation.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}niz en az {2} en fazla {1} karakter olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre onay")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor kontrol ediniz.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Kullanıcıyı yetkili olarak oluştur")]
        public bool Authorized { get; set; }

    }
}
