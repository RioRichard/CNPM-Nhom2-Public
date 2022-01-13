using Microsoft.AspNetCore.Mvc;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public class CategoryController : BaseController
    {
        
        public CategoryController(DataContext context) : base(context) { }

        [Route("/category/{id}")]
        [Route("/category/{id}/{page}")]

        public IActionResult Category(int id, int page=1)
        {
            ViewBag.Category = CategoryRepo.GetCategory(context);
            ViewBag.Currentpage = page;
            ViewBag.MaId = id;
            ViewBag.TotalPage = ProductRepo.CaculatedTotal(context, id);
            return View(ProductRepo.Get12ProductFollowFatherCategory(context,id,page));
        }

    }
}
