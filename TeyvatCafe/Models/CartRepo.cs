using System;
using System.Collections.Generic;
using System.Linq;

namespace TeyvatCafe.Models
{
    public class CartRepo
    {
        public static Cart CreateCart(DataContext context, string userId)
        {
            var newGuid = Helper.GenerateGuidCart(context);
            var newCart = new Cart()
            {
                IdAccount = userId,
                IdCart = newGuid,
                IsExpired = false,


            };
            context.Carts.Add(newCart);
            context.SaveChanges();
            return newCart;
        }
        public static void AddItemToCart(DataContext context, Guid idCart, Product product, int quantity)
        {
            var productCart = context.ProductCarts.FirstOrDefault(p => p.IdCart == idCart && p.IdProduct == product.IdProduct);
            if (productCart == null)
            {
                productCart = new ProductCart()
                {
                    IdCart = idCart,
                    IdProduct = (int)product.IdProduct,
                    PaymentPrice = product.Price,
                    Quantity = quantity,
                };
                context.ProductCarts.Add(productCart);
                context.SaveChanges();
            }
            else
            {
                productCart.Quantity += quantity;
                if (productCart.Quantity < 0)
                    productCart.Quantity = 0;
                context.Update(productCart);
                context.SaveChanges();
            }



        }
        public static bool AddItem(DataContext context, string userId, int idProduct, int quantity)
        {
            var product = context.Products.FirstOrDefault(p => p.IdProduct == idProduct);
            if (quantity > product.Stock)
                return false;
            var cart = context.Carts.FirstOrDefault(p => p.IdAccount == userId && p.IsExpired == false);
            if (cart == null)
            {
                cart = CreateCart(context, userId);
            }
            AddItemToCart(context, cart.IdCart, product, quantity);

            return true;
        }
        public static IEnumerable<ProductCart> GetAllCartItem(DataContext context, string userId)
        {
            var cart = context.Carts.FirstOrDefault(p => p.IdAccount == userId && p.IsExpired == false);
            if (cart == null)
                return null;
            var productCart = context.ProductCarts.Where(p => p.IdCart == cart.IdCart).ToList();
            if (productCart.Count == 0)
                return null;
            var product = context.Products.ToList();
            var result = (from proCart in productCart
                          join pro in product
                          on proCart.IdProduct equals (int)pro.IdProduct
                          select new ProductCart
                          {
                              IdCart = proCart.IdCart,
                              IdProduct = proCart.IdProduct,
                              Quantity = proCart.Quantity,
                              ProductName = pro.Name,
                              PaymentPrice = pro.Price,
                              UrlImage = pro.ImageUrl,
                              Stock = (int)pro.Stock

                          }).ToList();
            if (result.Count != 0)
                UpdateProductCart(context, result);
            return result;
        }
        static void UpdateProductCart(DataContext context, List<ProductCart> newUpdate)
        {
            var CartId = newUpdate.First().IdCart;
            var oldProductCart = context.ProductCarts.Where(p => p.IdCart == CartId).ToList();
            foreach (var item in newUpdate)
            {
                var update = oldProductCart.FirstOrDefault(p => p.IdProduct == item.IdProduct);
                update.Quantity = item.Quantity;
                context.ProductCarts.Update(update);
                context.SaveChanges();
            }
        }
        static Invoice NewInvoice(DataContext context, Cart cart, int idAddress)
        {
            int statusWaiting = 1;
            var newGuid = Helper.GenerateGuidInvoice(context);
            var newInvoice = new Invoice()
            {
                IdInvoice = newGuid,
                IdAddress = idAddress,
                IdCart = cart.IdCart,
                IdStatus = statusWaiting,
                DateCreated = DateTime.UtcNow
            };
            context.Invoices.Add(newInvoice);
            context.SaveChanges();
            return newInvoice;
        }
        static bool checkStock(DataContext context, Cart cart)
        {
            var productCart = context.ProductCarts.Where(p => p.IdCart == cart.IdCart);
            var product = context.Products.ToList();

            foreach (var item in productCart)
            {
                var check = product.First(p => p.IdProduct == item.IdProduct);
                if (check.Stock < item.Quantity)
                    return false;
            }

            return true;
        }
        public static bool ChargeCart(DataContext context, string userId, int idAddress)
        {
            var cart = context.Carts.First(p => p.IdAccount == userId && p.IsExpired == false);
            var checkProduct = checkStock(context, cart);
            if (checkProduct == false)
            {
                return false;
            }
            GetAllCartItem(context, userId); //update new price
            cart.IsExpired = true;
            NewInvoice(context, cart, idAddress);
            return true;

        }
        public static List<ProductCart> GetAllCartItemInInvoice(DataContext context, Guid IdCart)
        {

            var productCart = context.ProductCarts.Where(p => p.IdCart == IdCart).ToList();

            var product = context.Products.ToList();
            var result = (from proCart in productCart
                          join pro in product
                          on proCart.IdProduct equals (int)pro.IdProduct
                          select new ProductCart
                          {
                              IdCart = proCart.IdCart,
                              IdProduct = proCart.IdProduct,
                              Quantity = proCart.Quantity,
                              ProductName = pro.Name,
                              PaymentPrice = proCart.PaymentPrice,
                              UrlImage = pro.ImageUrl,
                              Total = proCart.Quantity*proCart.PaymentPrice


                          }).ToList();

            return result;
        }
        public static object GetTotalFollowProduct(DataContext context )
        {

            var productCart = GetAllCartExpired(context);
            var groupProcart = productCart.GroupBy(p => p.ProductName).Select(p => new { name = p.Key, total = p.Sum(p => p.Quantity) });
            var total = groupProcart.Sum(p => p.total);
            var result = groupProcart.OrderByDescending(p => p.total).Take(6);
            var returnObj = new { totals = total, results = result};
            return returnObj;
        }
        static List<ProductCart> GetAllCartExpired(DataContext context)
        {
            var productCart = new List<ProductCart>();
            var cartExpired = context.Carts.Where(p => p.IsExpired == true).ToList();
            foreach (var item in cartExpired)
            {
                productCart.AddRange(GetAllCartItemInInvoice(context, item.IdCart));
            }
            return productCart;
        }
        

    }
}
