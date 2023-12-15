using BookHub_Group5.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using BookHub_Group5.Models;
using BookHub_Group5.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookHub_Group5.Controllers
{
    public class UserController : Controller
    {
        private readonly BookHubDBContext bookHubDBContext;

        public UserController(BookHubDBContext bookHubDBContext)
        {
            this.bookHubDBContext = bookHubDBContext;
        }
        [HttpGet]

        //public async Task<IActionResult> Library()
        //{
        //    var sales = await bookHubDBContext.sales.ToListAsync();
        //    return View(sales);
        //}
        public async Task<IActionResult> Library()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                if (userEmailClaim != null)
                {
                    var userEmail = userEmailClaim.Value;
                    var userSales = await bookHubDBContext.sales.Where(s => s.UserEmail == userEmail).ToListAsync();
                    return View(userSales);
                }
            }

            return View(new List<sales>());
        }

        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult PendingBooks()
        {
            return View();
        }
        public IActionResult AccountDetails()
        {
            return View();
        }
        public IActionResult PurchaseHistory()
        {
            return View();
        }
    }
}