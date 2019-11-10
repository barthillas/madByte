using medBytePresentation.Models.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static medBytePresentation.Models.ApiModels;

namespace medBytePresentation.Services
{
    public interface IMedByteApiService
    {
        Task<dynamic> Login(LoginViewModel model);
        Task<dynamic> Register(RegisterViewModel model);
        Task<dynamic> SendPasswordRecoveryMail(ForgotPasswordViewModel model);
        Task<dynamic> ResetPassword(ResetPasswordViewModel model);
        Task<dynamic> GetMetaData();
        Task<dynamic> GetProuct(int id, string accessToken, string refreshToken, Func<string, Task> tokenRefreshed);
    }
}
