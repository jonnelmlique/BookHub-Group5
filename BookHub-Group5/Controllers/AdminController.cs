using Microsoft.AspNetCore.Mvc;
using BookHub_Group5.Models;
using BookHub_Group5.Models.Domain;
using BookHub_Group5.Data;
using Microsoft.EntityFrameworkCore;

namespace BookHub_Group5.Controllers
{
    public class AdminController : Controller
    {
        private readonly BookHubDBContext bookHubDBContext;

        public AdminController(BookHubDBContext bookHubDBContext)
        {
            this.bookHubDBContext = bookHubDBContext;
        }
        [HttpGet]
        public async Task<IActionResult> Books()
        {
            var books = await bookHubDBContext.books.ToListAsync();
            return View(books);
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        //public IActionResult Books()
        //{
        //    return View();
        //}
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


        [HttpPost]
        public async Task<IActionResult> Add(AddBooksViewModel addBooksRequest)
        {
            if (ModelState.IsValid)
            {
                var book = new books
                {
                    booktitle = addBooksRequest.booktitle,
                    author = addBooksRequest.author,
                    genre = addBooksRequest.genre,
                    publicationyear = addBooksRequest.publicationYear,
                    description = addBooksRequest.description,
                    coverimage = await ConvertFormFileToByteArray(addBooksRequest.CoverImage),
                    bookfile = await ConvertFormFileToByteArray(addBooksRequest.BookFile),
                    price = addBooksRequest.price
                };

                await bookHubDBContext.books.AddAsync(book);
                await bookHubDBContext.SaveChangesAsync();

                return RedirectToAction("AddBooks", "Admin");
            }

            // If the model state is not valid, return to the view with validation errors
            return View(addBooksRequest);
        }

        private async Task<byte[]> ConvertFormFileToByteArray(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpGet]
        public async Task<IActionResult> View(int bookid)
        {
          var books = await bookHubDBContext.books.FirstOrDefaultAsync(x => x.bookid == bookid);

            if (books != null)
            {
                var viewModel = new UpdateBooksViewModel()
                {
                    bookid = books.bookid,
                    booktitle = books.booktitle,
                    author = books.author,
                    genre = books.genre,
                    publicationyear = books.publicationyear,
                    description = books.description,
                    //coverimage = await ConvertFormFileToByteArray(books.coverimage),
                    //bookfile = await ConvertFormFileToByteArray(books.bookfile),
                    coverimage = books.coverimage, 
                    bookfile = books.bookfile,
                    price = books.price
                };
                if (viewModel.coverimage != null)
                {
                    var base64CoverImage = Convert.ToBase64String(viewModel.coverimage);
                    viewModel.CoverImageBase64 = $"data:image/png;base64,{base64CoverImage}";
                }
                
                if (viewModel.bookfile != null)
        {
            viewModel.BookFileBase64 = Convert.ToBase64String(viewModel.bookfile);
        }

                return View(viewModel);

            }
            return RedirectToAction("Books");
            //if (books == null)
        }


    }
}