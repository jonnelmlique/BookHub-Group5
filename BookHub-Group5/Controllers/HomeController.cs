using BookHub_Group5.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookHub_Group5.Models.Domain;
using BookHub_Group5.Data;
using Microsoft.EntityFrameworkCore;

namespace BookHub_Group5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookHubDBContext bookHubDBContext;

        public HomeController(ILogger<HomeController> logger, BookHubDBContext bookHubDBContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bookHubDBContext = bookHubDBContext ?? throw new ArgumentNullException(nameof(bookHubDBContext));
        }
        public async Task<IActionResult> Shop()
        {
            var books = await bookHubDBContext.books.ToListAsync();
            return View(books);
        }
        
        public async Task<IActionResult> Index()
        {
            var books = await bookHubDBContext.books.ToListAsync();
            return View(books);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> View(int bookid)
        {
            var books = await bookHubDBContext.books.FirstOrDefaultAsync(x => x.bookid == bookid);

            if (books != null)
            {
                var viewModel = new HomeBooks()
                {
                    bookid = books.bookid,
                    booktitle = books.booktitle,
                    author = books.author,
                    genre = books.genre,
                    publicationyear = books.publicationyear,
                    description = books.description,
                    coverimage = books.coverimage,
                    price = books.price
                };

                if (viewModel.coverimage != null)
                {
                    var base64CoverImage = Convert.ToBase64String(viewModel.coverimage);
                    viewModel.CoverImageBase64 = $"data:image/png;base64,{base64CoverImage}";
                }

                return View(viewModel);
            }

            return RedirectToAction("Shop");
        }


        [HttpGet]
        public async Task<IActionResult> BookDetails(int bookid)
        {
            var books = await bookHubDBContext.books.FirstOrDefaultAsync(x => x.bookid == bookid);

            if (books != null)
            {
                var bookDetailsViewModel = new BookDetailsViewModel()
                {
                    bookid = books.bookid,
                    booktitle = books.booktitle,
                    author = books.author,
                    genre = books.genre,
                    publicationyear = books.publicationyear,
                    description = books.description,
                    coverimage = books.coverimage,
                    bookfile = books.bookfile,
                    price = books.price
                };
                if (bookDetailsViewModel.coverimage != null)
                {
                    var base64CoverImage = Convert.ToBase64String(bookDetailsViewModel.coverimage);
                    bookDetailsViewModel.CoverImageBase64 = $"data:image/png;base64,{base64CoverImage}";
                }

                if (bookDetailsViewModel.bookfile != null)
                {
                    bookDetailsViewModel.BookFileBase64 = Convert.ToBase64String(bookDetailsViewModel.bookfile);
                }

                return View(bookDetailsViewModel);

            }
            return RedirectToAction("Shop");
            //if (books == null)
        }


    }
}