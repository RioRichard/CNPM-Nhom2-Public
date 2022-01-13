using System;
using System.Collections.Generic;
using System.Linq;

namespace TeyvatCafe.Models
{
    public class InvoiceRepo
    {
        public static Invoice GetInvoiceDetail(DataContext context, Guid IdInvoice)
        {
            var invoice = context.Invoices.FirstOrDefault(p => p.IdInvoice == IdInvoice);
            var productCart = CartRepo.GetAllCartItemInInvoice(context, invoice.IdCart);
            invoice.ProductCarts = productCart;
            invoice.Total = (int)productCart.Sum(p => p.Total);
            var status = context.Statuses.FirstOrDefault(p => p.IdStatus == invoice.IdStatus).StatusName;
            invoice.Status = status;
            var address = context.Addresses.FirstOrDefault(p => p.IdAddress == invoice.IdAddress);
            invoice.Address = address;
            return invoice;
            
        }
        public static List<Invoice> GetInvoices(DataContext context,string userId)
        {
            var invoices = GetAllInVoiceOfAccount(context, userId);
            var result = new List<Invoice>();
            foreach (var item in invoices)
            {
                result.Add(GetInvoiceDetail(context, item.IdInvoice));
            }
            return result;
        }
        static List<Invoice> GetAllInVoiceOfAccount(DataContext context, string userId)
        {
            var address = context.AccountAddresses.Where(p => p.IdAccount == userId).ToList();
            var user = context.Accounts.FirstOrDefault(p => p.IdAccount == userId).UserName;
            var invoices = context.Invoices.ToList();
            var result = (from add in address
                          join inc in invoices
                          on add.IdAddress equals inc.IdAddress
                          select inc).ToList();
            foreach (var item in result)
            {
                item.UserName = user;
            }
                          
            return result;
        }
        public static int TotalInvoice(List<Invoice> invoices, int findInThisMonth = 0)
        {
            if(findInThisMonth == 1)
            {
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                invoices.Where(p => p.DateCreated >= startDate && p.DateExpired <= endDate).ToList();
            }
            var total = invoices.Sum(p => p.Total);
            return total;
        }
        public static List<Account> TotalBuyFolowUser(DataContext context)
        {
            
            var result = context.Accounts.ToList();
            foreach (var item in result)
            {
                var invoice = GetInvoices(context, item.IdAccount);
                var total = TotalInvoice(invoice);
                item.TotalBought = total;
            }
            
           
            return result;
        }
        public static List<Invoice> GetAllInvoice(DataContext context) {

            var invoices = context.Invoices.ToList();
            var detailedInvoice = new List<Invoice>();
            foreach (var item in invoices)
            {
                detailedInvoice.Add(GetInvoiceDetail(context, item.IdInvoice));
            }
            
            return detailedInvoice;
        }
        public static bool EditInvoice(DataContext context, int StatusID , Guid IDInvoice)
        {
            var Invo = context.Invoices.FirstOrDefault(p => p.IdInvoice == IDInvoice);
            if (Invo == null)
            {
                return false;
            }
            else
            {
                Invo.IdStatus = StatusID;
               
                context.Invoices.Update(Invo);
                context.SaveChanges();
                return true;
            }
        }
        
    }
}
