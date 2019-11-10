using medBytePresentation.Models.AccountViewModels;
using Microsoft.AspNetCore.Http;

using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static medBytePresentation.Models.ApiModels;

namespace medBytePresentation.Services
{
    public class MedByteApiService: IMedByteApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public MedByteApiService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
      
        }

        public async Task<dynamic> Login(LoginViewModel model)
        {            
            var modelJson = System.Text.Json.JsonSerializer.Serialize(model, jsonSerializerOptions);
            var stringContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("MedByte");
            var response = await client.PostAsync("account/login", stringContent);

            return response;

        }

        public async Task<dynamic> Register (RegisterViewModel model)
        {
            var modelJson = JsonSerializer.Serialize(model, jsonSerializerOptions);
            var stringContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("MedByte");
            var response = await client.PostAsync("account/register", stringContent);

            return response;
        }

        public async Task<dynamic> GetMetaData()
        {       
            var client = _clientFactory.CreateClient("MedByte");
            var metaData = await client.GetAsync("product/getallmetadata");
            return metaData;
        }


        public async Task<dynamic> Authorize(AuthorizeViewModel model)
        {
            var modelJson = JsonSerializer.Serialize(model, jsonSerializerOptions);
            var stringContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("MedByte");
            var response = await client.PostAsync("account/register", stringContent);

            return response;
        }

        public async Task<dynamic> SendPasswordRecoveryMail(ForgotPasswordViewModel model)
        {
            var modelJson = JsonSerializer.Serialize(model, jsonSerializerOptions);
            var stringContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("MedByte");
            var response = await client.PostAsync("account/sendpasswordresetmail", stringContent);

            return response;
        }

        public async Task<dynamic> ResetPassword(ResetPasswordViewModel model)
        {
            var modelJson = JsonSerializer.Serialize(model, jsonSerializerOptions);
            var stringContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("MedByte");
            var response = await client.PostAsync("account/refreshpassword", stringContent);

            return response;

        }

        public async Task<dynamic> GetProuct(int id, string accessToken, string refreshToken, Func<string, Task> tokenRefreshed)
        {
            var policy = CreatesTokenRefreshPolicy(tokenRefreshed, accessToken, refreshToken);
        
            var response = await policy.ExecuteAsync(context =>
            {
                var token = accessToken;
                if (context.ContainsKey("Token"))
                {
                    token = context["Token"]?.ToString();
                }
               
                var client = _clientFactory.CreateClient("MedByte");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return client.GetAsync("product/GetById/" + id);
            }, new Dictionary<string, object>
        {
            {"Token", accessToken}
            
        });

            try
            {
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException )
            {
                return null;
            }
            
        }

        private AsyncRetryPolicy<HttpResponseMessage> CreatesTokenRefreshPolicy(Func<string, Task> tokenRefreshed,string token, string refreshToken)
        {
            var policy = Policy
                .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (result, retryCount, context) =>
                {
                    if (!String.IsNullOrEmpty(refreshToken) && !String.IsNullOrEmpty(token))
                    {                       
                        TokenModel model = new TokenModel()
                        {
                            RefreshToken = refreshToken,
                            Token= token
                        };
                   
                        var response =  await ResfreshToken(model);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            TokenModel tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(content);
                        if (tokenModel.Token != null)
                        {
                            context["Token"] = tokenModel.Token;
                            await tokenRefreshed(tokenModel.Token);
                        }
                      }
                    } 
                });
            return policy;
        }

        private async Task<dynamic> ResfreshToken(TokenModel model)
        {
            var modelJson = JsonSerializer.Serialize(model, jsonSerializerOptions);
            var stringContent = new StringContent(modelJson, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("MedByte");
            var httpResponse = await client.PostAsync("account/refresh", stringContent);
            
            if (httpResponse.IsSuccessStatusCode) { 
                var content = await httpResponse.Content.ReadAsStringAsync();

                return Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(content);
            }
            return null;
        }
    }
}
