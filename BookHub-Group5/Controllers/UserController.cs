using BookHub_Group5.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using BookHub_Group5.Models;
using BookHub_Group5.Models.Domain;

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
//        [HttpPost]
//        public async Task<IActionResult> Add(AddBooksViewModel addBooksRequest)
//        {
//            if (ModelState.IsValid)
//            {
//                var book = new books
//                {
//                    booktitle = addBooksRequest.booktitle,
//                    author = addBooksRequest.author,
//                    genre = addBooksRequest.genre,
//                    publicationyear = addBooksRequest.publicationYear,
//                    description = addBooksRequest.description,
//                    coverimage = await ConvertFormFileToByteArray(addBooksRequest.CoverImage),
//                    bookfile = await ConvertFormFileToByteArray(addBooksRequest.BookFile),
//                    price = addBooksRequest.price
//                };

//                await bookHubDBContext.books.AddAsync(book);
//                await bookHubDBContext.SaveChangesAsync();

//                return RedirectToAction("AddBooks", "Admin");
//            }

//            // If the model state is not valid, return to the view with validation errors
//            return View(addBooksRequest);
//        }

//        private async Task<byte[]> ConvertFormFileToByteArray(IFormFile file)
//        {
//            if (file == null)
//            {
//                return null;
//            }

//            using (var memoryStream = new MemoryStream())
//            {
//                await file.CopyToAsync(memoryStream);
//                return memoryStream.ToArray();
//            }
//        }
//    }
//}