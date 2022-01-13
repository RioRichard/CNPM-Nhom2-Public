using System.Collections.Generic;
using System.Linq;

namespace TeyvatCafe.Models
{
    public class CategoryRepo
    {
        
        public static List<Category> GetCategory(DataContext context)
        {
            var list = context.Categories.Where(p=>p.IsDelete==false).ToList();
            List<Category> tree = new List<Category>();
            Dictionary<int, Category> dict = new Dictionary<int, Category>();
            foreach (var item in list)
            {
                dict[(int)item.IDCategory] = item;
                if (item.FatherCategory is null)
                {
                    tree.Add(item);
                }
                else
                {
                    if (dict[item.FatherCategory.Value].ChildCategory is null)
                    {
                        dict[item.FatherCategory.Value].ChildCategory = new List<Category>();
                    }
                    dict[item.FatherCategory.Value].ChildCategory.Add(item);
                }
            }
            return tree;
        }
        public static Category AddCategory(DataContext context, string categoryName2, int? IDFather)
        {
            var newCategory= new Category()
            {
                CategoryName = categoryName2,
                FatherCategory= IDFather,
                IsDelete = false,
            };
            context.Categories.Add(newCategory);
            context.SaveChanges();
            return newCategory;
        }
        public static bool EditCategory(DataContext context, int cateID, string categoryName, int? IDFather1)
        {
            var Cate = context.Categories.FirstOrDefault(p => p.IDCategory == cateID);
            if (Cate == null)
            {
                return false;
            }
            else
            {
                Cate.CategoryName = categoryName;
                Cate.FatherCategory = IDFather1;
                context.Categories.Update(Cate);
                context.SaveChanges();
                return true;
            }
        }
    }
}
