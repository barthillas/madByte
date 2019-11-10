using medByteApi.Models.ViewModels;
using System.Collections.Generic;

namespace medByteApi.Services
{
    public partial interface IDbService
    {
        IEnumerable<ProductViewModel> GetAllProducts();
        IEnumerable<CategoryViewModel> GetAllCategories();
        ProductViewModel GetProductById(int id);
        //User Create(ProductViewModel product);
        //void Update(ProductViewModel product;
        //void Delete(int id);
        bool AddGenericAttribute(GenericAttributeViewModel model);
        GenericAttributeViewModel GetGenericAttribute(GenericAttributeViewModel model);
        bool DeleteGenericAttribute(GenericAttributeViewModel model);

    }
}
