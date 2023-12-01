using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookHub_Group5.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public async Task Account()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        //    {
        //        claim.Issuer,
        //        claim.OriginalIssuer,
        //        claim.Type,
        //        claim.Value

        //    });

        //    //return Json(claims);

        //    return RedirectToAction("Library", "User", new {area=""});
        //}
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var userEmailClaim = result.Principal.Identities.FirstOrDefault()?.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (userEmailClaim != null)
            {
                if (userEmailClaim.Equals("bookhubofficialcontact@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Dashboard", "Admin", new { area = "" });
                }
                else
                {
                    return RedirectToAction("Library", "User", new { area = "" });
                }
            }

            return RedirectToAction("Error", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            string googleLogoutUrl = "https://www.google.com/accounts/Logout?continue=https://accounts.google.com/Logout";

            return Redirect(googleLogoutUrl);
            //await HttpContext.SignOutAsync();
            //return View("Login");
        }
    }
}
