using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace TeyvatCafe.Models
{
    public class ProductRepo
    {
        public static List<Product> Get4ProductByCate(DataContext context, int idCategory)
        {
            var categories = context.Categories.Where(p => p.IDCategory == idCategory).ToList();
            var products = context.Products.ToList();
            var productCategories = context.ProductCategories.ToList();
            var output = (from pro in products
                          join jtable in productCategories
                          on pro.IdProduct equals jtable.IdProduct
                          join cate in categories
                          on jtable.IdCategory equals cate.IDCategory
                          select new Product
                          {
                              IdProduct = pro.IdProduct,
                              ImageUrl = pro.ImageUrl,
                              IsDelete = pro.IsDelete,
                              Description = pro.Description,
                              Name = pro.Name,
                              Price = pro.Price,
                              SKU = pro.SKU,
                              Stock = pro.Stock
                          }).Where(p=>p.IsDelete==false).Take(4).ToList();
            return output;

        }

        public static List<Product> Get12Product(DataContext context, int idCategory)
        {
            var categories = context.Categories.Where(p => p.IDCategory == idCategory).ToList();
            var products = context.Products.ToList();
            var productCategories = context.ProductCategories.ToList();
            var output = (from pro in products
                          join jtable in productCategories
                          on pro.IdProduct equals jtable.IdProduct
                          join cate in categories
                          on jtable.IdCategory equals cate.IDCategory
                          select new Product
                          {
                              IdProduct = pro.IdProduct,
                              ImageUrl = pro.ImageUrl,
                              IsDelete = pro.IsDelete,
                              Description = pro.Description,
                              Name = pro.Name,
                              Price = pro.Price,
                              SKU = pro.SKU,
                              Stock = pro.Stock

                          }).Take(12).ToList();
            return output;

        }
        public static List<ProductAttribute> GetAttribute(DataContext context, int idProducts)
        {
            var  products = context.Products.Where(p => p.IdProduct == idProducts).ToList();
            var attribute = context.Attributes.ToList();
            var productAttributes = context.ProductAttributes.ToList();
            var output = (from pro in products
                          join jtable in productAttributes
                          on pro.IdProduct equals jtable.IdProduct
                          join cate in attribute
                          on jtable.IdAttribute equals cate.IdAttribute
                          select new ProductAttribute
                          {
                              Value= jtable.Value,
                              AttributeName= cate.AttributeName,
                              IdAttribute= jtable.IdAttribute,
                              IdProduct= jtable.IdProduct

                          }).ToList();
            return output;
        }
        public static List<Product> Get12ProductFollowFatherCategory(DataContext context, int idCategory, int page)
        {
            var categoriesContext = context.Categories.ToList();
            var products = context.Products.ToList();
            var productCategoriesContext = context.ProductCategories.ToList();
            var productCategories = productCategoriesContext.FirstOrDefault(p => p.IdCategory == idCategory);
            List<Category> categories = new List<Category>();
            if (productCategories == null)
            {
                categories = categoriesContext.Where(p => p.FatherCategory == idCategory).ToList();
            }
            else
                categories = categoriesContext.Where(p => p.IDCategory == idCategory).ToList();

            var output = (from pro in products
                          join jtable in productCategoriesContext
                          on pro.IdProduct equals jtable.IdProduct
                          join cate in categories
                          on jtable.IdCategory equals cate.IDCategory
                          select new Product
                          {
                              IdProduct = pro.IdProduct,
                              ImageUrl = pro.ImageUrl,
                              IsDelete = pro.IsDelete,
                              Description = pro.Description,
                              Name = pro.Name,
                              Price = pro.Price,
                              SKU = pro.SKU,
                              Stock = pro.Stock
                          }).Where(p => p.IsDelete == false).Skip((page - 1) * 12).Take(12).ToList();
            return output;


        }
        public static int CaculatedTotal (DataContext context, int idCategory)
        {
            int ItemOfPage = 12;
            var categoriesContext = context.Categories.ToList();
            var products = context.Products.ToList();
            var productCategoriesContext = context.ProductCategories.ToList();
            var productCategories = productCategoriesContext.FirstOrDefault(p => p.IdCategory == idCategory);
            List<Category> categories = new List<Category>();
            if (productCategories == null)
            {
                categories = categoriesContext.Where(p => p.FatherCategory == idCategory).ToList();
            }
            else
                categories = categoriesContext.Where(p => p.IDCategory == idCategory).ToList();

            var TotolofItem = (from pro in products
                          join jtable in productCategoriesContext
                          on pro.IdProduct equals jtable.IdProduct
                          join cate in categories
                          on jtable.IdCategory equals cate.IDCategory
                          select new Product
                          {
                              IdProduct = pro.IdProduct,
                              ImageUrl = pro.ImageUrl,
                              IsDelete = pro.IsDelete,
                              Description = pro.Description,
                              Name = pro.Name,
                              Price = pro.Price,
                              SKU = pro.SKU,
                              Stock = pro.Stock
                          }).Where(p => p.IsDelete == false).Count();
            int RealTotal = 0;
            var Mod = TotolofItem%ItemOfPage;
            var Ceiling = TotolofItem/ItemOfPage;

            if (Mod > Ceiling)
            {
                RealTotal = Ceiling +1;
            }
            else
            {
                RealTotal = Ceiling;
            }
            return RealTotal;
        }

        public static Product GetProduct(DataContext context,int idProduct)
        {
            var product = context.Products.FirstOrDefault(p => p.IdProduct == idProduct);
            product.Category = GetCategoriesOfProduct(context, idProduct);
            product.ProductAttributes = GetAttributeOfProduct(context, idProduct);
            return product;
            
            
        }
        static List<ProductAttribute> GetAttributeOfProduct(DataContext context,int productId)
        {
            var productAttribute = context.ProductAttributes.Where(p => p.IdProduct == productId).ToList();
            var attribute = context.Attributes.ToList();
            var resultPA = (from pA in productAttribute
                            join attr in attribute
                            on pA.IdAttribute equals (int)attr.IdAttribute
                            select new ProductAttribute
                            {
                                IdAttribute = pA.IdAttribute,
                                IdProduct = pA.IdProduct,
                                AttributeName = attr.AttributeName,
                                Value = pA.Value
                            }).ToList();
            return resultPA;
        }
        static List<ProductCategory> GetCategoriesOfProduct(DataContext context, int idProduct)
        {
            var productCategories = context.ProductCategories.Where(p => p.IdProduct == idProduct).ToList();
            var categories = context.Categories.ToList();
            var resultPC = (from pC in productCategories
                            join cate in categories
                            on pC.IdCategory equals (int)cate.IDCategory
                            select new ProductCategory
                            {
                                IdCategory = pC.IdCategory,
                                IdProduct = pC.IdProduct,
                                CategoryName = cate.CategoryName
                            }).ToList();
            return resultPC;
        }
        public static bool EditProduct(DataContext context, int proID, string productName, int productPrice, int productStock, string productDes, int cttr, int[] attrID, string[] productAttr1, IFormFile UploadEdit, string productpath)
        {
            var Pros = context.Products.FirstOrDefault(p => p.IdProduct == proID);
            if (Pros == null)
            {
                return false;
            }
            else
            {
                var result = Helper.FileUpload(UploadEdit, productpath);
                if(result != null)
                {
                    Helper.DeleteFile(Pros.ImageUrl, productpath);
                    Pros.ImageUrl = result.ImageUrl;

                }


                Pros.Name = productName;
                Pros.Price = productPrice;
                Pros.Stock = productStock;
                Pros.Description = productDes;
                 
                context.Products.Update(Pros);
                context.SaveChanges();
            }
            var proCategory = context.ProductCategories.FirstOrDefault(p => p.IdProduct == proID);
            context.ProductCategories.Remove(proCategory);
            context.SaveChanges();
            proCategory.IdCategory = cttr;
            context.ProductCategories.Add(proCategory);
            context.SaveChanges();
            var proAttr = context.ProductAttributes.Where(p => p.IdProduct == proID).ToList();
            if (proAttr.Count == 0)
            {
                for (int i = 0; i < attrID.Length; i++)
                {
                    var attrProd = new ProductAttribute()
                    {
                        IdProduct = proID,
                        IdAttribute = attrID[i],
                        Value = productAttr1[i]
                    };
                    context.ProductAttributes.Add(attrProd);
                    context.SaveChanges();
                }
            }
            else
            {
                for (int i = 0; i < attrID.Length; i++)
                {
                    var x = proAttr.FirstOrDefault(p => p.IdAttribute == attrID[i]);
                    if (x == null)
                    {
                        var attrProd = new ProductAttribute()
                        {
                            IdProduct = proID,
                            IdAttribute = attrID[i],
                            Value = productAttr1[i]
                        };
                        context.ProductAttributes.Add(attrProd);
                        context.SaveChanges();
                    }
                    else
                    {
                        context.ProductAttributes.Remove(x);
                        context.SaveChanges();
                        if (!string.IsNullOrEmpty(productAttr1[i]))
                        {
                            x.IdAttribute = attrID[i];
                            x.Value = productAttr1[i];
                            context.ProductAttributes.Add(x);
                            context.SaveChanges();
                        }                       
                    }

                }

            }
            return true;
        }

        public static Product AddProduct(DataContext context, string productName2, int productPrice2, int productStock2, string productDes2, int idCate, int[] attr, string[] attrValue, IFormFile f, string productpath)
        {
            var result = Helper.FileUpload(f, productpath);
            var newProduct = new Product()
            {
                Name = productName2,
                Price = productPrice2,
                Stock = productStock2,
                Description = productDes2,
                ImageUrl=result.ImageUrl,
                IsDelete = false,
            };
            context.Products.Add(newProduct);
            context.SaveChanges();
            var productCate = new ProductCategory()
            {
                IdProduct = (int)newProduct.IdProduct,
                IdCategory = idCate,
            };
            context.ProductCategories.Add(productCate);
            context.SaveChanges();
            for (int i = 0; i < attr.Length; i++)
            {
                if (!string.IsNullOrEmpty(attrValue[i]))
                {
                    var productAttr = new ProductAttribute()
                    {
                        IdProduct = (int)newProduct.IdProduct,
                        IdAttribute = attr[i],
                        Value= attrValue[i],
                    };
                    context.ProductAttributes.Add(productAttr);
                    context.SaveChanges();
                }
            }

            //for
            return newProduct;
        }


    }
}
