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
        //public async Task<IActionResult> Library()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

        //        if (userEmailClaim != null)
        //        {
        //            var userEmail = userEmailClaim.Value;
        //            var userSales = await bookHubDBContext.sales.Where(s => s.UserEmail == userEmail).ToListAsync();
        //            return View(userSales);
        //        }
        //    }

        //    return View(new List<sales>());
        //}
        public async Task<IActionResult> Library(string searchString)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                if (userEmailClaim != null)
                {
                    var userEmail = userEmailClaim.Value;
                    var query = bookHubDBContext.sales.Where(s => s.UserEmail == userEmail).AsQueryable();

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        searchString = searchString.ToLower();
                        query = query.Where(s =>
                            s.BookTitle.ToLower().Contains(searchString) ||
                            s.Author.ToLower().Contains(searchString) ||
                            s.Genre.ToLower().Contains(searchString)
                        );
                    }

                    var userSales = await query.ToListAsync();
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
        public async Task<IActionResult> PurchaseHistory()
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

        [HttpGet]
        public async Task<IActionResult> UserLibrary(int SaleId)
        {
            var sales = await bookHubDBContext.sales.FirstOrDefaultAsync(x => x.SaleId == SaleId);

            if (sales != null)
            {
                var viewModel = new LibraryBooks()
                {
                    BookId = sales.BookId,
                    BookTitle = sales.BookTitle,
                    Author = sales.Author,
                    Genre = sales.Genre,
                    PublicationYear = sales.PublicationYear,
                    Description = sales.Description,
                    CoverImage = sales.CoverImage,
                    BookFile = sales.BookFile,

                    Price = sales.Price
                };

                if (viewModel.CoverImage != null)
                {
                    var base64CoverImage = Convert.ToBase64String(viewModel.CoverImage);
                    viewModel.CoverImageBase64 = $"data:image/png;base64,{base64CoverImage}";
                }
                if (viewModel.BookFile != null)
                {
                    viewModel.BookFileBase64 = Convert.ToBase64String(viewModel.BookFile);
                }

                return View(viewModel);
            }

            return RedirectToAction("Library");
        }


    }
}