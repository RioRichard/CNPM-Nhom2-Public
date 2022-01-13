using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public class AdminAuthController : BaseController
    {
        
        string AdminCookieName = "TeyvatStaff";
        public AdminAuthController(DataContext context, IConfiguration configuration) : base(context,configuration) {
            
        
        }
        



        public IActionResult LogIn()
        {
            if (Request.Cookies[AdminCookieName] != null)
                return Redirect("/");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(AuthModel obj)
        {
            AccountRepo.ReturnAuthentication claims;
            var result = AccountStaffRepo.LoginAccount(context, obj, out claims);
            switch (result)
            {
                case 0:
                    var checkCesendTooSoon = accountRepo.ResendEmail(context, claims.IdUser, claims.Email, claims.expiredTokenTime);
                    break;
                case 1: 
                    await HttpContext.SignInAsync("Staff_Scheme", claims.principal, claims.properties);
                    break;

                default:
                    break;

            }
            return Json(result);
        }


        [Authorize(AuthenticationSchemes = "Staff_Scheme")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync("Staff_Scheme");
            Response.Cookies.Delete(AdminCookieName);
            return Redirect("/");
        }



        [HttpPost]
        public IActionResult ForgotPassword(string idStaff)
        {
            
            var result = accountStaffRepo.ForgotPassword(context, idStaff);
            return Json(result);
        }
        [Route("/adminauth/resetpassword/{flag}/{token}")]
        public IActionResult ResetPassword(int flag,string token)
        {
            if (Request.Cookies[AdminCookieName] != null)
                return Redirect("/admin/product");
            var check = context.AccountStaffs.FirstOrDefault(p => p.Token == token);
            if (check == null)
                return Redirect(Helper.url404);
            var checkTime = DateTime.Compare(check.ExpiredTokenTime, DateTime.UtcNow);
            if (checkTime < 0)
            {
                return Redirect("/adminauth/sendmailagain");
            }
            ViewBag.token = token;
            ViewBag.flag = flag;
            return View();
        }
        
        [HttpPost]
        public IActionResult ResetPasswords(int flag,string token, string Pass)
        {
            

            var account = context.AccountStaffs.FirstOrDefault(p => p.Token == token);
            var result = AccountStaffRepo.ResetPassword(context, account, Pass);
            if (flag == 1)
            {
                account.IsConfirmed = true;
                context.AccountStaffs.Update(account);
                context.SaveChanges();
            }
            return Json(result);



        }
        [HttpPost]
        public IActionResult AddStaff(string staffEmail, string userName, List<Guid> roleId)
        {

            var obj = new StaffModel()
            {
                Email = staffEmail,
                UserName = userName,
                RoleId = roleId
            };
            var result = accountStaffRepo.AddStaff(context, obj);
            

            return Json(result);
        }
        public IActionResult GetStaffInID(string idStaff)
        {
            var result = AccountStaffRepo.GetAccountStaff(context, idStaff);
            return Json(result);
        }
        [Route("/adminAuth/ConfirmAccount/{Token}")]
        public IActionResult ConfirmAccount(string Token)
        {
            var result = accountStaffRepo.ConfirmMail(context, Token);
            return Redirect(result);

        }
        public IActionResult EmailConfirmCompleted()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendConfirmMailAgain(string idStaff)
        {
            var Staff = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == idStaff);
            var result = accountStaffRepo.ResendEmail(context, Staff);
            return Json(result);
        }
        public IActionResult SendMailAgain()
        {
            return View();
        }
    }
}
