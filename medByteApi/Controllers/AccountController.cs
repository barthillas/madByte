using medByteApi.Entities;
using medByteApi.Helpers;
using medByteApi.Models.ViewModels;
using medByteApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace medByteApi.Controllers
{
    [ApiController]
    [Route("apiv1/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IDbService _DbService;
        private readonly IMailSender _mailSender;
        private readonly ApplicationCacheManager _cacheManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IDbService DbService,
            IMailSender mailSender,
            ApplicationCacheManager cacheManager
            )
        {
            _mailSender = mailSender;
            _DbService = DbService;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _cacheManager = cacheManager;
        }


        [HttpPost]
        public async Task<object> CreateRole([FromBody] RoleModel model)
        {
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole { Name = model.Name, NormalizedName = model.Name, ConcurrencyStamp = Guid.NewGuid().ToString() });
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }



        [HttpPost]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded) // Error Validasyonlarını yönlendir.
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                return await GenerateJwtToken(model.Email, appUser);
            }

            return new ObjectResult(new
            {
                Model = model,
                Error = "INVALID_LOGIN_ATTEMPT"

            });

        }

        [HttpGet]
        public async Task<object> LogOut()
        {
            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear();
            return Ok();
        }

        [HttpPost]
        public async Task<object> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            var asdasd = _DbService.GetAllProducts();

            if (result.Succeeded) // Error Validasyonlarını yönlendir.
            {
                if (model.Authorized)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "admin");
                }
                else
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "user");
                }
                await _signInManager.SignInAsync(user, false);
                return await GenerateJwtToken(model.Email, user);
            }

            return new ObjectResult(new
            {
                result,
                Error = result.Errors.ToString()
            });
        }




        private async Task<object> GenerateJwtToken(string email, IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            string joinedRoles = string.Join(", ", roles);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(ClaimTypes.Role, joinedRoles)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(50);//AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            var userToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken().ToString();
            _cacheManager.Set(user.Id, refreshToken);
            return new ObjectResult(new
            {
                Token = userToken,
                RefreshToken = refreshToken
            });
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        [HttpPost]
        public async Task<object> Refresh([FromBody] RefreshTokenModel model)
        {
            var principal = GetPrincipalFromExpiredToken(model.Token);
            var Email = principal.Claims.First().Value;
            var user = _userManager.Users.SingleOrDefault(r => r.Email == Email);

            var savedRefreshToken = _cacheManager.Get<string>(user.Id);
            if (savedRefreshToken != model.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var asdasd = await GenerateJwtToken(Email, user);
            return asdasd;
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [HttpPost]
        public async Task<object> SendPasswordResetMail([FromBody] ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user.Email != model.Email)
                return new ObjectResult(new
                {
                    resetToken = "",
                    Succeeded = false,
                    Error = "User does not exist"
                });
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            GenericAttributeViewModel genericAttributeViewModel = new GenericAttributeViewModel()
            {
                EntityId = user.Id,
                KeyGroup = "User",
                Key = "PasswordToken",
                Value = resetToken
            };
            bool isSuccess = _DbService.AddGenericAttribute(genericAttributeViewModel);
            var urlEncodedToken = System.Net.WebUtility.UrlEncode(resetToken);
            StringBuilder body = new StringBuilder("<p>Merhaba,</p>");
            body.Append("<p>");
            body.Append("<br>MedByte hesabının şifresini aşağıdaki linki kullanarak yenileyebilirsin.");
            body.Append("</p>");
            body.Append("<p>");
            body.Append($"<p><a href=\"http://localhost:5000/Account/ResetPassword?t={urlEncodedToken}\">Şifre yenileme sayfasına yönlendirilmek için tıklayın.</a> </p>");
            body.Append("</p>");
            body.Append("<p>Sevgiler, MadByte işe giriş projesi.</p>");
            body.Append("<p>");
            body.Append("<br>");
            body.Append("</p>");
            await _mailSender.SendEmailAsync("MedByte | Şifre Yenileme", model.Email, body.ToString());
            return new ObjectResult(new
            {
                resetToken,
                Succeeded = true,
            });

        }

        [HttpPost]
        public async Task<object> RefreshPassword([FromBody] RefreshPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ObjectResult(new
                {
                    Succeeded = false,
                    Error = "User doesn't exist."
                });
            }


            GenericAttributeViewModel genericAttributeViewModel = new GenericAttributeViewModel()
            {
                EntityId = user.Id,
                KeyGroup = "User",
                Key = "PasswordToken",
                Value = model.Code
            };
            GenericAttributeViewModel attributeViewModel = _DbService.GetGenericAttribute(genericAttributeViewModel);

            if (attributeViewModel != null)
            {
                var asd = await _userManager.RemovePasswordAsync(user);
                var dsa = await _userManager.AddPasswordAsync(user, model.Password);

                _DbService.DeleteGenericAttribute(attributeViewModel);

                return new ObjectResult(new
                {
                    Succeeded = true
                });
            }
            return new ObjectResult(new
            {
                Succeeded = false,
                Error = ""
            });
        }

       
    }
}
