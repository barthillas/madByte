using medBytePresentation.Helpers;
using medBytePresentation.Models;
using medBytePresentation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static medBytePresentation.Models.ApiModels;

namespace medBytePresentation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IMedByteApiService _medByteApiService;
        private readonly ILogger _logger;

        public HomeController(
            IMedByteApiService medByteApiService,
            ILoggerFactory loggerFactory)
        {
            _medByteApiService = medByteApiService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            var httpResponse = await _medByteApiService.GetMetaData();
            var content = await httpResponse.Content.ReadAsStringAsync();
            MetaData MetaData = JsonConvert.DeserializeObject<MetaData>(content);

            Tools newInstance = new Tools();
            var tree = newInstance.BuildTree(MetaData.Categories.ToList());

            MetaData.Categories = tree.Where(i => i.CategoryParentId == 0).ToList();
            return View(MetaData);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = Role.Product_View)]
        [HttpGet("{ProductName}/{id}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(int id, string ProductName)
        {

            var sessionToken = HttpContext.Session.GetString("Token");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
          
            var httpResponse = await _medByteApiService.GetProuct(id, sessionToken, refreshToken, async token =>{
                HttpContext.Session.SetString("Token", token);
                sessionToken = token;
            });

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                ProductViewModel product = JsonConvert.DeserializeObject<ProductViewModel>(content);
                if (product == null)
                {
                    return this.NotFound();
                }
                string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(product.ProductName);

                if (!string.Equals(friendlyTitle, ProductName, StringComparison.Ordinal))
                {

                    return this.RedirectToRoutePermanent("GetProduct", new { id, title = friendlyTitle });
                }
                return View("Product", product);
            }
            return View();
        }
    }
}


