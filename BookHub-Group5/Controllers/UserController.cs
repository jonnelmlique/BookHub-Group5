using Microsoft.AspNetCore.Mvc;

namespace BookHub_Group5.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Library()
        {
            return View();
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
