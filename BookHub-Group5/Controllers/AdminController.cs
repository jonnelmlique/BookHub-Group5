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
        //public async Task<IActionResult> Books()
        //{
        //    var books = await bookHubDBContext.books.ToListAsync();
        //    return View(books);
        //}

        //public async Task<IActionResult> Books(string searchString)
        //{
        //    var query = bookHubDBContext.books.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        query = query.Where(b => b.booktitle.ToLower().Contains(searchString.ToLower()));
        //    }

        //    var books = await query.ToListAsync();
        //    return View(books);
        //}

        public async Task<IActionResult> Books(string searchString)
        {
            var query = bookHubDBContext.books.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(b =>
                    b.booktitle.ToLower().Contains(searchString) ||
                    b.author.ToLower().Contains(searchString) ||
                    b.genre.ToLower().Contains(searchString) ||
                    b.publicationyear.ToString().Contains(searchString) ||
                    b.price.ToString().Contains(searchString)
                );
            }

            var books = await query.ToListAsync();
            return View(books);
        }



        [HttpGet]

        public IActionResult Dashboard()
        {

            int totalBooks = bookHubDBContext.books.Count();

            decimal totalSales = bookHubDBContext.sales.Sum(s => s.Price);
            ViewBag.TotalSales = totalSales;

            ViewBag.TotalBooks = totalBooks;


            return View();
        }
        public IActionResult AddBooks()
        {
            return View();
        }
        //public async Task<IActionResult> Sales()
        //{
        //    return View();
        //}
        //public async Task<IActionResult> Sales()
        //{
        //    decimal totalSales = bookHubDBContext.sales.Sum(s => s.Price);
        //    ViewBag.TotalSales = totalSales;

        //    var sales = await bookHubDBContext.sales.ToListAsync();
        //    return View(sales);
        //}
        public async Task<IActionResult> Sales(string searchString, DateTime? saleDate)
        {
            var query = bookHubDBContext.sales.AsQueryable();

            // Apply the search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(s =>
                    s.UserEmail.ToLower().Contains(searchString) ||
                    s.BookTitle.ToLower().Contains(searchString) ||
                    s.Price.ToString().Contains(searchString)
                );
            }

            if (saleDate.HasValue)
            {
                query = query.Where(s => s.SaleDate.Date == saleDate.Value.Date);
            }

            var filteredSales = await query.ToListAsync();

            decimal totalSales = filteredSales.Sum(s => s.Price);
            ViewBag.TotalSales = totalSales;

            return View(filteredSales);
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
                try
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

                    TempData["SuccessMessage"] = "Book added successfully.";

                    return RedirectToAction("AddBooks", "Admin");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the book.";
                    return RedirectToAction("AddBooks", "Admin");

                }
            }

            TempData["ErrorMessage"] = "An error occurred while adding the book.";

            return RedirectToAction("AddBooks", "Admin");

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

                return await Task.Run(() => View("View", viewModel));

            }
            return RedirectToAction("Books");
            //if (books == null)
        }


        [HttpPost]
        public async Task<IActionResult> View(UpdateBooksViewModel model)
        {
            var books = await bookHubDBContext.books.FindAsync(model.bookid);

            if (books != null)
            {
                books.booktitle = model.booktitle;
                books.author = model.author;
                books.genre = model.genre;
                books.publicationyear = model.publicationyear;
                books.description = model.description;
                books.price = model.price;

                if (model.coverimage != null)
                {
                    books.coverimage = model.coverimage;
                }

                if (model.bookfile != null)
                {
                    books.bookfile = model.bookfile;
                }

                await bookHubDBContext.SaveChangesAsync();

                return RedirectToAction("Books");

            }
            return RedirectToAction("Books");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UpdateBooksViewModel model)
        {
            var books = await bookHubDBContext.books.FindAsync(model.bookid);
            if (books != null)
            {
                bookHubDBContext.books.Remove(books);
                await bookHubDBContext.SaveChangesAsync();
                return RedirectToAction("Books");

            }
            return RedirectToAction("Books");


        }
    }
}




