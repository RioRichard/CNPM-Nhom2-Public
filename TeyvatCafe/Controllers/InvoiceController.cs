using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public class InvoiceController : BaseController
    {
        public InvoiceController(DataContext context) : base(context)
        {
        }

        
        [Authorize]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "Staff_Scheme,User_Scheme")]

        public IActionResult InvoiceDetail(string idInvoice)
        {
            var result = InvoiceRepo.GetInvoiceDetail(context, Guid.Parse(idInvoice));

            return Json(result);
        }
        
        
        
    }
}
