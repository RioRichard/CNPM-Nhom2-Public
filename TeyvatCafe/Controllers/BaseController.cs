using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TeyvatCafe.Models;

namespace TeyvatCafe.Controllers
{
    public abstract class BaseController : Controller
    {
        protected DataContext context;

        protected AccountRepo accountRepo;
        protected AccountStaffRepo accountStaffRepo;
        public BaseController(DataContext context)
        {
            this.context = context;
            
        }
        public BaseController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
           
            this.accountRepo = new AccountRepo(configuration);
            this.accountStaffRepo = new AccountStaffRepo(configuration);


        }

    }
}
