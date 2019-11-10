using medByteApi.Entities;
using medByteApi.Models.DB;
using medByteApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace medByteApi.Services
{


    public class DbService : IDbService
    {
        public IConfiguration Configuration { get; }
        public aspnetmedByteApiContext _dbContext { get; set; } 

        public DbService(IConfiguration configuration, aspnetmedByteApiContext dbContext)
        {
            Configuration = configuration;
            _dbContext = dbContext;
        }

        public  IEnumerable<ProductViewModel> GetAllProducts()
        {

            var ProductsWithCategories = _dbContext.ProductsWithCategory.FromSqlRaw("[dbo].[Products_With_Category]").ToList();     

            List<ProductViewModel> result = new List<ProductViewModel>();
            foreach (var item in ProductsWithCategories)
            {
                ProductViewModel product = new ProductViewModel()
                {
                    CategoryName = item.CategoryName,
                    IsActive = item.IsActive,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl,
                    ProductCategoryId = item.ProductCategoryId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName
                };

                result.Add(product);
            }
            return result.AsEnumerable(); ;
        }

        public IEnumerable<CategoryViewModel> GetAllCategories()
        {
            var CategoryTree = _dbContext.CategoryTree.FromSqlRaw("[dbo].[CategoryTree]").ToList();
            List<CategoryViewModel> result = new List<CategoryViewModel>();
            foreach (var item in CategoryTree)
            {
                CategoryViewModel product = new CategoryViewModel()
                {
                    CategoryName = item.CategoryName,
                    CategoryId =item.CategoryId,
                    Level =item.LeafLevel,
                    CatVar =item.CatVar,
                    CategoryParentId =item.CategoryParentId
                };

                result.Add(product);
            }
            return result.AsEnumerable(); 
          
        }

        public bool AddGenericAttribute(GenericAttributeViewModel model)
        {
            GenericAttribute genericAttribute = new GenericAttribute() {
                EntityId = model.EntityId,
                KeyGroup = model.KeyGroup,
                Key=model.Key,
                Value=model.Value                
            };
            _dbContext.GenericAttribute.Add(genericAttribute);
            _dbContext.SaveChanges();
            return true;
        }

        public GenericAttributeViewModel GetGenericAttribute(GenericAttributeViewModel model)
        {
            GenericAttributeViewModel result = new GenericAttributeViewModel();
            var genericAttribute = _dbContext.GenericAttribute.FirstOrDefault(x => x.KeyGroup == model.KeyGroup && x.EntityId == model.EntityId && x.Key == model.Key&&x.Value==model.Value);
            if (genericAttribute != null)
            {
                return new GenericAttributeViewModel() {
                    EntityId = model.EntityId,
                    KeyGroup = model.KeyGroup,
                    Key = model.Key,
                    Value = model.Value
                };
            }
            return new GenericAttributeViewModel();
        }

        public bool DeleteGenericAttribute(GenericAttributeViewModel model)
        {
            var genericAttribute = _dbContext.GenericAttribute.FirstOrDefault(x => x.KeyGroup == model.KeyGroup && x.EntityId == model.EntityId && x.Key == model.Key);
            _dbContext.GenericAttribute.Remove(genericAttribute);
            _dbContext.SaveChanges();

            return true;
        }
          
        public ProductViewModel GetProductById(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.ProductId == id);
            if (product == null) return null;
            var category = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == product.ProductCategoryId);
            string productCategory = "";
            if (category == null) productCategory = category.CategoryName;
            return new ProductViewModel()
            {
                ProductId =product.ProductId,
                Description = product.Description,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                ProductCategoryId = product.ProductCategoryId,
                ProductName = product.ProductName,
                CategoryName = productCategory

            };
           
        }
    }
}