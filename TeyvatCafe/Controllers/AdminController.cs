using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme")]

    public class AdminController : BaseController
    {
        IConfiguration configuration;
        string productPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "productImg");
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "icon");


        public AdminController(DataContext context, IConfiguration configuration) : base(context)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var Role = new List<string>();
            var listRole = User.Claims.Where(p => p.Type == ClaimTypes.Role).ToList();
            foreach (var item in listRole)
            {
                Role.Add(item.Value);
            }
            ViewBag.role = listRole;
            return View(context.AccountStaffs.FirstOrDefault(p=>p.IDStaff == userId));
        }
        [HttpPost]
        public IActionResult Index(string fullname, bool? gender)
        {

            var userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

            AccountStaffRepo.ChangeUserInfo(context, userId, fullname, gender);

            return Redirect("/admin");
        }


        [Route("/admin/attribute")]
        [Route("/admin/attribute/1/{page}")]
        public IActionResult Attribute(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.Attributes.Where(p => p.IsDelete == false).Count());
            ViewBag.Currentpage = page;
            ViewBag.Products = context.Products.ToList();
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            return View(context.Attributes.Where(p => p.IsDelete == false).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }

        [Route("/admin/product")]
        [Route("/admin/product/2/{page}")]
        public IActionResult Product(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.Products.Where(p=>p.IsDelete == false).Count());
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            ViewBag.Categorys = context.Categories.Where(p => p.IsDelete == false).ToList();
            ViewBag.Attributes = context.Attributes.Where(p=>p.IsDelete == false).ToList();
            return View(context.Products.Where(p => p.IsDelete == false).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }

        [Route("/admin/category")]
        [Route("/admin/category/3/{page}")]
        public IActionResult Category(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.Categories.Where(p => p.IsDelete == false).Count());
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            ViewBag.Fathers = context.Categories.ToList();
            ViewBag.Products = context.Products.ToList();
            ViewBag.Attributes = context.Attributes.ToList();
            return View(context.Categories.Where(p => p.IsDelete == false).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList()) ;
        }
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme", Roles = "SuperAdmin,Admin")]

        [Route("/admin/StaffManagement")]
        [Route("/admin/StaffManagement/8/{page}")]
        public IActionResult StaffManagement(int page =1)
        {
            int ItemOfPage = 10;
            var accountStaffs = AccountStaffRepo.GetStaffID(context).Where(p => p.IsBanned == false).ToList();

            int total = accountStaffs.Count();
            var result = accountStaffs.Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList();
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            var roles = context.Roles.ToList();
            ViewBag.RoleSS = roles;
            ViewBag.RoleChange =roles.Where(p=>p.RoleName != "SuperAdmin").ToList();


            return View(result);
        }
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme", Roles = "SuperAdmin,Admin")]
        [Route("/admin/BannedStaff")]
        [Route("/admin/BannedStaff/9/{page}")]
        public IActionResult BannedStaff(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.AccountStaffs.Where(p => p.IsBanned == true).Count());
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            return View(AccountStaffRepo.GetStaffID(context).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }
        [Route("/admin/deletedattribute")]
        [Route("/admin/deletedattribute/4/{page}")]
        public IActionResult DeletedAttribute(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.Attributes.Where(p => p.IsDelete == true).Count());
            ViewBag.Currentpage = page;
            ViewBag.Products = context.Products.ToList();
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            return View(context.Attributes.Where(p => p.IsDelete == true).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }
        [Route("/admin/deletedcategory")]
        [Route("/admin/deletedcategory/5/{page}")]
        public IActionResult DeletedCategory(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.Categories.Where(p => p.IsDelete == true).Count());
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            ViewBag.Fathers = context.Categories.ToList();
            ViewBag.Products = context.Products.ToList();
            ViewBag.Attributes = context.Attributes.ToList();
            return View(context.Categories.Where(p => p.IsDelete == true).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }
        [Route("/admin/deletedproduct")]
        [Route("/admin/deletedproduct/6/{page}")]
        public IActionResult DeletedProduct(int page = 1)
        {
            int ItemOfPage = 10;
            int total = (context.Products.Where(p => p.IsDelete == true).Count());
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            ViewBag.Categorys = context.Categories.ToList();
            ViewBag.Attributes = context.Attributes.ToList();
            return View(context.Products.Where(p => p.IsDelete == true).Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }
        [HttpPost]
        public IActionResult EditAttribute(int attrId, string attributeName)
        {
            var result = AttributeRepo.EditAttribute(context, attrId, attributeName);
            return Json(result);
        }
        [HttpPost]
        public IActionResult AddAtribute(string attributeName2)
        {
            AttributeRepo.AddAttribute(context, attributeName2);

            return Json(true);
        }
        [HttpPost]
        public IActionResult AddCategory(string categoryName2, int? IDFather)
        {
            CategoryRepo.AddCategory(context, categoryName2, IDFather);

            return Json(true);
        }
        [HttpPost]
        public IActionResult EditCategory(int IdCate, string categoryName, int? IDFather1)
        {
            var result = CategoryRepo.EditCategory(context, IdCate, categoryName, IDFather1);
            return Json(result);
        }


        public IActionResult getProduct(int pID)
        {
            var result =ProductRepo.GetProduct(context, pID);
            return Json(result);
        }

        
        [HttpPost]
        public IActionResult EditProduct(int proID, string productName, int productPrice, int productStock, string productDes, int cttr, int[] attrID, string[] productAttr1,IFormFile UploadEdit)
        {
            var result = ProductRepo.EditProduct(context, proID, productName, productPrice, productStock, productDes, cttr, attrID, productAttr1, UploadEdit, productPath);
            
            return Json(result);
        }
        [HttpPost]
        public IActionResult AddProduct(string productName2, int productPrice2, int productStock2, string productDes2, int idCate, int[] attr, string[] attrValue, IFormFile imgUp)
        {
            ProductRepo.AddProduct(context, productName2, productPrice2, productStock2, productDes2, idCate, attr, attrValue, imgUp, productPath);
            
            return Json(true);
        }
        [HttpPost]

        public IActionResult DeleteProduct(int pdID)
        {
            var prod = context.Products.FirstOrDefault(p => p.IdProduct == pdID);
            if (prod == null)
            {
                return Json(false);
            }
            else
            {
                prod.IsDelete = true;
                context.Products.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        [HttpPost]

        public IActionResult DeleteCategory(int ctgrID2)
        {
            var prod = context.Categories.FirstOrDefault(p => p.IDCategory == ctgrID2);
            if (prod == null)
            {
                return Json(false);
            }
            else
            {
                prod.IsDelete = true;
                context.Categories.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        [HttpPost]
        public IActionResult DeleteAttribute(int attrID2)
        {
            var prod = context.Attributes.FirstOrDefault(p => p.IdAttribute == attrID2);
            if (prod == null)
            {
                return Json(false);
            }
            else
            {
                prod.IsDelete = true;
                context.Attributes.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme", Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public IActionResult BannedTrashStaff(string sID2)
        {

            var prod = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == sID2);
            if (prod == null)
            {
                return Json(false);
            }
            else
            {
                var idSA = context.Roles.FirstOrDefault(p => p.RoleName == "SuperAdmin");
                var staffRole = context.StaffRoles.FirstOrDefault(p => p.IDStaff == prod.IDStaff && p.IdRole.Equals(idSA.IDRole));
                if(staffRole == null)
                {
                    prod.IsBanned = true;
                    context.AccountStaffs.Update(prod);
                    context.SaveChanges();
                    return Json(true);
                }
                
                
                return Json(false);
            }
        }
        public IActionResult RestoreCategory(int ctgrID)
        {
            var prod = context.Categories.FirstOrDefault(p => p.IDCategory == ctgrID);
            if (prod == null)
            {
                return Json(true);
            }
            else
            {
                prod.IsDelete = false;
                context.Categories.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        public IActionResult RestoreProduct(int pID)
        {
            var prod = context.Products.FirstOrDefault(p => p.IdProduct == pID);
            if (prod == null)
            {
                return Json(true);
            }
            else
            {
                prod.IsDelete = false;
                context.Products.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        public IActionResult RestoreAttribute(int attrID)
        {
            var prod = context.Attributes.FirstOrDefault(p => p.IdAttribute == attrID);
            if (prod == null)
            {
                return Json(true);
            }
            else
            {
                prod.IsDelete = false;
                context.Attributes.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme", Roles = "SuperAdmin,Admin")]
        [HttpPost]

        public IActionResult RestoreStaff(string sID)
        {
            var prod = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == sID);
            if (prod == null)
            {
                return Json(true);
            }
            else
            {
                prod.IsBanned = false;
                context.AccountStaffs.Update(prod);
                context.SaveChanges();
                return Json(prod);
            }
        }
        public IActionResult Chart()
        {
            return View();
        }
        
        public IActionResult DashboardBar()
        {

            var accounts = InvoiceRepo.TotalBuyFolowUser(context);
            var result = accounts.OrderByDescending(p => p.TotalBought).Select(p => new { p.UserName,p.TotalBought });
            return Json(result);
        }
        public IActionResult DashboardPie()
        {
            var result = CartRepo.GetTotalFollowProduct(context);
            return Json(result);
        }
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme", Roles = "SuperAdmin,Admin")]

        public IActionResult Setting()
        {
            return View();
        }
        public IActionResult Test()
        {
            configuration["ImageUrlLogo"] = "logo-TGCF-224.png";
            return Json(configuration["ImageUrlLogo"]);
        }
        [HttpPost]
        public IActionResult TestFile(IFormFile logoFile)
        {
            var result = Helper.FileUpload(logoFile, logoPath);
            Helper.UpdateAppSetting("ImageUrlLogo", result.ImageUrl);
            var oldImage = configuration["ImageUrlLogo"];
            Helper.DeleteFile(oldImage, logoPath);


            return Json(result);
        }
        public IActionResult UploadBanner(IFormFile bannerFile)
        {
            string jsonBannerKey = "ImageUrlBanner";
            var oldImage = configuration["ImageUrlBanner"];
            var result = Helper.FileUpload(bannerFile, logoPath);
            Helper.UpdateAppSetting(jsonBannerKey, result.ImageUrl);
            Helper.DeleteFile(oldImage, logoPath);
            return Json(result);
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePass(string currentPass, string newPass)
        {
            var userid = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var result = AccountStaffRepo.ChangePassword(context, userid, currentPass, newPass);
            return Json(result);
        }
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme", Roles = "SuperAdmin,Admin")]

        public IActionResult ChangeRole(string userId, List<Guid> roleId)
        {
            AccountStaffRepo.ChangeRole(context, userId, roleId);
            return Json(true);
            
        }
        [Route("/admin/invoicedetail")]
        [Route("/admin/invoicedetail/7/{page}")]
        public IActionResult Invoicedetail(int page = 1)
        {
            int ItemOfPage = 10;
            var result = InvoiceRepo.GetAllInvoice(context);
            int total = result.Count;
            ViewBag.Currentpage = page;
            if (total % ItemOfPage == 0)
            {
                ViewBag.TotalPage = (total / ItemOfPage);
            }
            else
            {
                ViewBag.TotalPage = (total / ItemOfPage) + 1;
            }
            ViewBag.Status = context.Statuses.ToList();
            
            
            return View(result.Skip((page - 1) * ItemOfPage).Take(ItemOfPage).ToList());
        }
        [HttpPost]
        public IActionResult EditInvoicedetail(int SelectStatus, Guid IdInvoice)
        {

            InvoiceRepo.EditInvoice(context, SelectStatus, IdInvoice);

            return Json(true);

        }

    }
}
