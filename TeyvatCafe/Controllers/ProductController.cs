using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(DataContext context) : base(context) { }

        [Route("/product/{id}")]
        public IActionResult Product(int id)
        {
            ViewBag.PA = ProductRepo.GetAttribute(context, id);
            return View(context.Products.FirstOrDefault(p => p.IdProduct == id));
        }

    }
}
