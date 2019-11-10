using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace medBytePresentation.Models
{
    public class ApiModels
    {

        public class TokenModel
        {
            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("refreshToken")]
            public string RefreshToken { get; set; }
        }
     
        public class CategoryViewModel
        {

            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int CategoryParentId { get; set; }

            public string CatVar { get; set; }

            public int Level { get; set; }
            private ICollection<CategoryViewModel> _Categories;
            public ICollection<CategoryViewModel> Categories
            {
                get { return this._Categories; }
                set
                {
                    this._Categories = value;

                    //if (this._Categories != null)
                    //{
                    //var list = GetChildRecursive(this._Categories);
                    //if (!(list.Count() == 0)&&list!=null)
                    //{
                    //        if (this.Childrens == null)
                    //        {
                    //            this.Childrens = list;
                    //        }
                    //        else
                    //        {
                    //            this.Childrens.Concat(list).ToList();
                    //        }

                    //}
                    //}
                }
            }
            //public string Family { get; set; }
            public List<CategoryViewModel> _Childrens { get; set; }
            public List<CategoryViewModel> Childrens
            {
                get { return this._Childrens; }
                set
                {
                    this._Childrens = value;

                    this.ChildrenIds = string.Join("-", this._Childrens.Select(i => i.CategoryId).ToArray());// this._Childrens.Aggregate((i, j) => i.CategoryId.ToString() + " - " + j.CategoryId.ToString());

                }
            }

            public string ChildrenIds { get; set; }

            private List<CategoryViewModel> GetChildRecursive(ICollection<CategoryViewModel> obj)
            {
                //if (null == obj&&!obj.Any())
                //    return new List<CategoryViewModel>();

                List<CategoryViewModel> children = new List<CategoryViewModel>();
                foreach (CategoryViewModel child in obj.ToList())
                {
                    if (child.Categories == null)
                    {

                        children.Add(child);
                        //     continue;
                    }
                    else
                    {
                        var childrens = GetChildRecursive(child.Categories);
                        if (childrens != null) children.AddRange(childrens);
                    }
                }

                return children;
            }

        }




        public class ProductViewModel
        {
            [JsonProperty("productId")]
            public int ProductId { get; set; }

            [JsonProperty("productName")]
            public string ProductName { get; set; }

            [JsonProperty("productCategoryId")]
            public int ProductCategoryId { get; set; }

            [JsonProperty("imageUrl")]
            public string ImageUrl { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("isActive")]
            public bool IsActive { get; set; }

            [JsonProperty("categoryName")]
            public string CategoryName { get; set; }

        }


        public class MetaData
        {
            public IEnumerable<ProductViewModel> Products { get; set; }

            public IEnumerable<CategoryViewModel> Categories { get; set; }
        }
    }
}

