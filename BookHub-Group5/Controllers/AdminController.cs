using Microsoft.AspNetCore.Mvc;

namespace BookHub_Group5.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Books()
        {
            return View();
        }
        public IActionResult AddBooks()
        {
            return View();
        }
        public IActionResult Sales()
        {
            return View();
        }
        public IActionResult Accounts()
        {
            return View();
        }
    }
}
