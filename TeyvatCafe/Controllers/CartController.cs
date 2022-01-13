using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        public CartController(DataContext context) : base(context) { }

        [HttpPost]
        public IActionResult AddItemToCart(int idProduct, int Quantity)
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var result = CartRepo.AddItem(context, userId, idProduct, Quantity);
            if (result == true)
                TempData["messageAddItem"] = "Thêm vào giỏ hàng thành công";
            else
                TempData["messageAddItem"] = "Thêm vào giỏ hàng không thành công";

            return Redirect($"/product/{idProduct}");
        }
        [HttpPost]
        public IActionResult ChangeQuantityCart(string CartID, int ProductId, int Quantity)
        {
            var proCart = context.ProductCarts.FirstOrDefault(p => p.IdCart == Guid.Parse(CartID) && p.IdProduct == ProductId);
            proCart.Quantity = Quantity;
            if (proCart.Quantity == 0)
            {
                context.ProductCarts.Remove(proCart);
            }
            else
                context.ProductCarts.Update(proCart);
            context.SaveChanges();
            return Json(Quantity);
        }
        [HttpPost]
        public IActionResult DeleteProductCart(string CartID, int ProductId)
        {
            var proCart = context.ProductCarts.FirstOrDefault(p => p.IdCart == Guid.Parse(CartID) && p.IdProduct == ProductId);
            context.ProductCarts.Remove(proCart);
            context.SaveChanges();
            return Json(true);
        }
        [HttpPost]
        public IActionResult ChargeCart(int IdAddress)
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var result = CartRepo.ChargeCart(context, userId,IdAddress);
            return Json(result);
        }



    }
}
