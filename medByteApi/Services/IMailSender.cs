using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Services
{
    public interface IMailSender
    {
        Task SendEmailAsync(string Name, string Email, string Message);
        
    }
}
