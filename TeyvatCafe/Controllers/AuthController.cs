    using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public class AuthController : BaseController
    {
        
        public AuthController(DataContext context, IConfiguration configuration) : base(context,configuration) {
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            return View();
        }
        [HttpPost]
        public IActionResult Register(AuthModel obj)
        {
            //emailExisted = 0;
            //userExisted = -1;
            //noExisted = 1;
            var result = accountRepo.RegisterAccount(context, obj);

            return Json(result);
        }
        [Route("/auth/ConfirmAccount/{Token}")]
        public IActionResult ConfirmAccount(string Token)
        {
            var result = accountRepo.ConfirmMail(context, Token);
            return Redirect(result);

        }
        
       

        public IActionResult LogIn()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(AuthModel obj)
        {
            AccountRepo.ReturnAuthentication claims;
            var result = AccountRepo.LoginAccount(context, obj, out claims);
            if(result == 0)
            {
                var checkCesendTooSoon = accountRepo.ResendEmail(context, claims.IdUser, claims.Email, claims.expiredTokenTime);
                return Json(checkCesendTooSoon);

            }
            if (result == 1)
            {
                await HttpContext.SignInAsync("User_Scheme", claims.principal, claims.properties);
            }
            return Json(result);
            
        }


        [Authorize(AuthenticationSchemes = "User_Scheme")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            
            var result = accountRepo.ForgotPassword(context, email);
            return Json(result);
        }
        [Route("/auth/resetpassword/{token}")]
        public IActionResult ResetPassword(string token)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            var check = context.Accounts.FirstOrDefault(p => p.Token == token);
            if(check == null)
                return Redirect("/404");
            var checkTime = DateTime.Compare(check.ExpiredTokenTime, DateTime.UtcNow);
            if (checkTime < 0)
            {
                TempData["messageForgotPassword"] = "Đường dẫn đã hết hạn. Vui lòng gửi lại email";
                return Redirect("/auth/forgotPassword");
            }
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string Pass)
        {
            var account = context.Accounts.FirstOrDefault(p => p.Token == token);
            var result = AccountRepo.ResetPassword(context, account, Pass);
            return Json(result);
                

            
        }
        public IActionResult EmailConfirmCompleted()
        {
            return View();
        }
        //[Route("/auth/SendMailAgain/{Token}")]
        public IActionResult SendMailAgain()
        {
            return View();
        }
    }
}
