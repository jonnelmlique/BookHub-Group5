using Microsoft.AspNetCore.Mvc;

namespace BookHub_Group5.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
