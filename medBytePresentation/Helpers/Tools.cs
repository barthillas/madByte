using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using medBytePresentation.Models;
using static medBytePresentation.Models.ApiModels;

namespace medBytePresentation.Helpers
{
    public class Tools
    {


        public IEnumerable<CategoryViewModel> BuildTree(List<CategoryViewModel> model)
        {
            var topLevel = model.Max(x => x.Level);

            var onject = model;
            var returnModel = new List<CategoryViewModel>();
            for (int i = topLevel; i > 0; i--)
            {
                var asd = onject.Where(x => x.Level == i);
                foreach (var item in asd)
                {

                    CategoryViewModel asdaa = onject.FirstOrDefault(x => x.CategoryId == item.CategoryParentId);

                    if (asdaa.Childrens == null)
                    {
                        asdaa.Childrens = new List<CategoryViewModel>();
                    }

                    asdaa.Childrens.Add(item);
                    var oneLevelDown = asd.Where(x => x.CategoryParentId == item.CategoryParentId);
                    if (oneLevelDown.Any())
                    {

                        asdaa.Categories = (ICollection<CategoryViewModel>)oneLevelDown.ToList();
                    }

                    //.Childrens.Add(item);
                    if (item.Childrens != null)
                    {
                        asdaa.Childrens.AddRange(item.Childrens);
                    }
                    asdaa.ChildrenIds = string.Join("-", asdaa.Childrens.Select(i => i.CategoryId).ToArray()); //asdaa.string.Join(",", persons.Select(p => p.FirstName))
                    returnModel.Add(asdaa);
                }
            }

            return onject;
        }

        //private static IEnumerable<CategoryViewModel> BuildTree(List<CategoryViewModel> items)
        //{
        //    //items.Reverse();
        //    items.ForEach(i => i.Categories = items.Where(ch => ch.CategoryParentId == i.CategoryId).ToList());

        //    return items.Where(i => i.CategoryParentId == 0).ToList();
        //}


    }
}
