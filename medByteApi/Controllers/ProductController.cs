using medByteApi.Entities;
using medByteApi.Models.ViewModels;
using medByteApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace medByteApi.Controllers
{
    
    [ApiController]
    [Route("apiv1/[controller]/[action]")]
    public class ProductController :Controller
    {      
        private readonly IDbService _DbService;

        public ProductController(
          
            IDbService DbService
            )
        {
            _DbService = DbService;            
        }

        //[AllowAnonymous]
        //[HttpPost("authenticate")]
        //public IActionResult Authenticate([FromBody]AuthenticateModel model)
        //{
        //    var user = _userService.Authenticate(model.Username, model.Password);

        //    if (user == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    return Ok(user);
        //}
        [Authorize(Roles = Role.ProductView)]
        [HttpGet("{id}")]
        public ProductViewModel GetById(int id)
        {
           

            var product = _DbService.GetProductById(id);

    

            return product;
        }

        [AllowAnonymous]
        [HttpGet]
        public  IEnumerable<dynamic> GetAllProducts()
        {
            var products =  _DbService.GetAllProducts();
            return products;
        }
        //  [Authorize(Roles = Role.ProductView)]
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<dynamic> GetAllCategories()
        {
            var categories = _DbService.GetAllCategories();
            return categories;
        }


        [AllowAnonymous]
        [HttpGet]
        public MetaData GetAllMetaData()
        {
            var categories = _DbService.GetAllCategories();
            var products = _DbService.GetAllProducts();

            return new MetaData() { 
                Categories = categories,
                Products = products
            };
        }

        //public User GetById(int id)
        //{
        //    var user = _users.FirstOrDefault(x => x.Id == id);
        //    return user.WithoutPassword();
        //}

        //[Authorize(Roles = Role.Admin)]
        //[HttpGet("{id}")]
        //public IActionResult GetById(int id)
        //{
        //    // only allow admins to access other user records
        //    var currentUserId = int.Parse(User.Identity.Name);
        //    if (id != currentUserId && !User.IsInRole(Role.Admin))
        //        return Forbid();

        //    var user = _userService.GetById(id);

        //    if (user == null)
        //        return NotFound();

        //    return Ok(user);
        //}
    }
}
