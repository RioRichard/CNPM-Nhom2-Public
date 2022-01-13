using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AccountController : BaseController
    {
        

        public AccountController(DataContext context) : base(context) { 
            

        }

        public IActionResult Index()
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var account = context.Accounts.FirstOrDefault(p => p.IdAccount == userId);
            return View(account);
        }
        [HttpPost]
        public IActionResult Index(string fullname, bool? gender)
        {
            
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            AccountRepo.ChangeUserInfo(context, userId, fullname, gender);

            return Redirect("/account");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string currentPass, string newPass)
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            var result = AccountRepo.ChangePassword(context, userId, currentPass, newPass);
            if(result == false)
            {
                TempData["messageChangePass"] = "Mật khẩu cũ không đúng";

            }
            else
                TempData["messageChangePass"] = "Đổi mật khẩu thành công";
            return View();
        }
        public IActionResult Address()
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            return View(AddressRepo.GetAddressByUser(context,userId));
        }
        public IActionResult Cart()
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            var result = CartRepo.GetAllCartItem(context, userId);
            var address = AddressRepo.GetAddressByUser(context, userId);

            ViewBag.Address = address;
            ViewBag.AddressCount = address.Count;
            
            if (result == null)
            {
                var fakeList = new List<ProductCart>();
                return View(fakeList);

            }
            
            return View(result);
        }
        [HttpPost]
        public IActionResult AddAddress(string address, string phone, string receiver, bool isDef)
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            AddressRepo.AddAccount(context, userId, address, phone, receiver, isDef);

            return Json(true);
        }
        [HttpPost]
        public IActionResult ChangeDefalut(int AddID)
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            AddressRepo.SetDefault(context, userId, AddID);
            return Json(true);
        }
        public IActionResult Invoice()
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var result = InvoiceRepo.GetInvoices(context, userId);
            return View(result);
        }
        
        
    }
}
