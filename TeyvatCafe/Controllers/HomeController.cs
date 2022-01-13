using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public class HomeController : BaseController
    {
  
        public HomeController(DataContext context) : base(context) { }
        public IActionResult Index()
        {
            ViewBag.Category = CategoryRepo.GetCategory(context);
            var main = context.Products.Where(p=>p.IsDelete==false).ToList();
            ViewBag.Cafe = main.Where(p => p.Name.Contains("Cà phê")).Take(4);
            ViewBag.DungCuCafe = main.Where(p => p.Name.Contains("Bình") || p.Name.Contains("Gas")).Take(4);
            var outputView = main.Where(p => p.Name.Contains("Máy")).Take(4).ToList();

            return View(outputView);
            
        }
        [Route("/404")]
        public IActionResult NotFound404()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Search(string search)
        {
            ViewBag.Category = CategoryRepo.GetCategory(context);

            var result = context.Products.Where(p => p.Name.Contains(search) && p.IsDelete==false).Take(12).ToList();
            return View(result);
        }
        public IActionResult AboutUS()
        {
            return View();
        }

    }

}
