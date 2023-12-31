﻿using BookHub_Group5.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookHub_Group5.Models.Domain;
using BookHub_Group5.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayPal.Api;
using System.Net.Mail;
using System.Net;

namespace BookHub_Group5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookHubDBContext bookHubDBContext;
        //private readonly string paypalClientId = "AVdRCS1RC4uiAHXCxIXZdWam2I6BTOAdS_v5IuFc2SMD52cbjxcV_aqOMa_orlQAN8yLET3rIWQvP_rG";
        //private readonly string paypalClientSecret = "EIlTE-EDVzDyrxQSXkVYAH06-PzU8QhWR9teoSajTqGGM8AidRx2Nny8DzcNzxgayjaeFv9l4MeEMwwt";
        public HomeController(ILogger<HomeController> logger, BookHubDBContext bookHubDBContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bookHubDBContext = bookHubDBContext ?? throw new ArgumentNullException(nameof(bookHubDBContext));
        }
        public async Task<IActionResult> Shop(string searchString)
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

        public async Task<IActionResult> Index()
        {
            var books = await bookHubDBContext.books.ToListAsync();


            return View(books);
        }
        public IActionResult Product()
        {
            return View();
        }
        public IActionResult About()
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
        public async Task<IActionResult> Product(int bookid)
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
        }
        [HttpGet]
        public async Task<IActionResult> BuyBook(int bookid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in to proceed with the purchase.";
                return RedirectToAction("Shop");
            }

            var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (userEmailClaim == null)
            {
                TempData["ErrorMessage"] = "Email not found. Please log in first.";
                return RedirectToAction("Shop");
            }

            var book = await bookHubDBContext.books.FirstOrDefaultAsync(x => x.bookid == bookid);

            if (book != null)
            {
                var apiContext = GetPayPalApiContext();

                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        currency = "PHP",
                        total = book.price.ToString("0.00")
                    },
                    description = book.booktitle,
                    item_list = new ItemList
                    {
                        items = new List<Item>
                        {
                            new Item
                            {
                                name = book.booktitle,
                                currency = "PHP",
                                price = book.price.ToString("0.00"),
                                quantity = "1",
                                description = book.booktitle,
                                sku = book.bookid.ToString()
                            }
                        }
                    }
                }
            },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = Url.Action("PaymentSuccess", "Home", new { bookId = book.bookid }, Request.Scheme),
                        cancel_url = Url.Action("PaymentCancel", "Home", null, Request.Scheme)
                    }
                };

                var createdPayment = payment.Create(apiContext);

                return Redirect(createdPayment.GetApprovalUrl());
            }

            return RedirectToAction("Shop");
        }

        private APIContext GetPayPalApiContext()
        {
            var paypalMode = "sandbox";
            var apiContext = new APIContext(new OAuthTokenCredential(GetPayPalClientId(), GetPayPalClientSecret()).GetAccessToken());

            apiContext.Config = new Dictionary<string, string> { { "mode", paypalMode } };

            return apiContext;
        }

        private string GetPayPalClientId()
        {
            return "AVdRCS1RC4uiAHXCxIXZdWam2I6BTOAdS_v5IuFc2SMD52cbjxcV_aqOMa_orlQAN8yLET3rIWQvP_rG";
        }

        private string GetPayPalClientSecret()
        {
            return "EIlTE-EDVzDyrxQSXkVYAH06-PzU8QhWR9teoSajTqGGM8AidRx2Nny8DzcNzxgayjaeFv9l4MeEMwwt";
        }
        //public async Task<IActionResult> PaymentSuccess(int bookId, string paymentId, string token, string payerId)
        //{
        //    var book = await bookHubDBContext.books.FirstOrDefaultAsync(x => x.bookid == bookId);

        //    if (book != null)
        //    {
        //        var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

        //        if (userEmailClaim != null)
        //        {
        //            var saleRecord = new sales
        //            {
        //                BookId = book.bookid,
        //                UserEmail = userEmailClaim.Value,
        //                SaleDate = DateTime.Now,
        //                Price = book.price,

        //                BookTitle = book.booktitle,
        //                Author = book.author,
        //                Genre = book.genre,
        //                PublicationYear = book.publicationyear,
        //                Description = book.description,
        //                CoverImage = book.coverimage,
        //                BookFile = book.bookfile
        //            };

        //            bookHubDBContext.sales.Add(saleRecord);
        //            await bookHubDBContext.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Email not found. Please log in first.";
        //            return RedirectToAction("Shop");
        //        }
        //    }

        //    return Content("Payment successful! Thank you for buying the book with ID " + bookId);

        //}

        public async Task<IActionResult> PaymentSuccess(int bookId, string paymentId, string token, string payerId)
        {
            var book = await bookHubDBContext.books.FirstOrDefaultAsync(x => x.bookid == bookId);

            if (book != null)
            {
                var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                if (userEmailClaim != null)
                {
                    var saleRecord = new sales
                    {
                        BookId = book.bookid,
                        UserEmail = userEmailClaim.Value,
                        SaleDate = DateTime.Now,
                        Price = book.price,

                        BookTitle = book.booktitle,
                        Author = book.author,
                        Genre = book.genre,
                        PublicationYear = book.publicationyear,
                        Description = book.description,
                        CoverImage = book.coverimage,
                        BookFile = book.bookfile
                    };

                    bookHubDBContext.sales.Add(saleRecord);
                    await bookHubDBContext.SaveChangesAsync();

                    return RedirectToAction("Shop");
                }
                else
                {
                    TempData["ErrorMessage"] = "Email not found. Please log in first.";
                    return RedirectToAction("Shop");
                }
            }

            return RedirectToAction("Shop");
        }
        [HttpPost]
        public async Task<IActionResult> SendContactEmail(string senderEmail, string subject, string message)
        {
            try
            {
                // Update with your SMTP server details
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "bookhubofficialcontact@gmail.com";
                string smtpPassword = "onzfuuhzstvorksa";

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add("bookhubofficialcontact@gmail.com");

                    await client.SendMailAsync(mailMessage);
                }

                TempData["SuccessMessage"] = "Email sent successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                TempData["ErrorMessage"] = "An error occurred while sending the email.";
                return RedirectToAction("Index");
            }

        }

    }
}
