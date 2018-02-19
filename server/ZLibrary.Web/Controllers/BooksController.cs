using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZLibrary.API;
using ZLibrary.Model;
using System.Collections.Generic;
using System.Linq;
using ZLibrary.Web.Controllers.Items;
using ZLibrary.Web.Validators;
using ZLibrary.Web.Extensions;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ZLibrary.Web
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookService bookService;
        private readonly IAuthorService authorService;
        private readonly IPublisherService publisherService;
        private readonly IReservationService reservationService;
        private readonly ILoanService loanService;

        public BooksController(IBookService bookService, IAuthorService authorService, IPublisherService publisherService, IReservationService reservationService, ILoanService loanService)
        {
            this.bookService = bookService;
            this.authorService = authorService;
            this.publisherService = publisherService;
            this.reservationService = reservationService;
            this.loanService = loanService;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var books = await bookService.FindAll();
            return Ok(books.ToBookViewItems());
        }


        [HttpGet("{id:long}", Name = "FindBook")]
        public async Task<IActionResult> FindById(long id)
        {
            var book = await bookService.FindById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book.ToBookViewItem());
        }

        [HttpDelete("{id:long}", Name = "DeleteBook")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await bookService.Delete(id);
                return NoContent();
            }
            catch (BookDeleteException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody]BookDTO value)
        {
            var validationContext = new ValidationContext(bookService, authorService, publisherService);
            var bookValidator = new BookValidator(validationContext);
            var validationResult = bookValidator.Validate(value);

            if (validationResult.HasError)
            {
                return BadRequest(validationResult.ErrorMessage);
            }

            var book = await bookService.FindById(value.Id);

            if (book == null && value.Id != 0)
            {
                return NotFound($"Nenhuma livro encontrada com o ID: {value.Id}.");
            }

            if (book == null)
            {
                book = new Book();
            }
            book.Title = value.Title;
            book.Synopsis = value.Synopsis;
            book.PublicationYear = value.PublicationYear;
            book.Isbn = validationResult.GetResult<Isbn>();
            book.Publisher = validationResult.GetResult<Publisher>();
            book.Authors = validationResult.GetResult<List<BookAuthor>>();
            book.NumberOfCopies = value.NumberOfCopies;
            book.CoverImageKey = value.CoverImageKey;

            foreach (var bookAuthor in book.Authors)
            {
                bookAuthor.Book = book;
                bookAuthor.BookId = book.Id;
            }

            try
            {
                await bookService.Save(book);

                return Ok(book.ToBookViewItem());
            }
            catch (BookSaveException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("search/", Name = "FindBookBy")]
        public async Task<IActionResult> FindBy([FromBody]SearchParametersDTO value)
        {
            var orderBy = (SearchOrderBy)Enum.ToObject(typeof(SearchOrderBy), value.OrderByValue);
            var bookSearchParameter = new BookSearchParameter(value.Keyword)
            {
                OrderBy = orderBy
            };
            var books = await bookService.FindBy(bookSearchParameter);
            var booksDTO = books.ToBookViewItems();
            foreach (var book in booksDTO)
            {
                var reservations = await reservationService.FindByBookId(book.Id);
                var reservationsDTO = reservations.ToReservationViewItems();
                foreach (var reservation in reservationsDTO)
                {
                    var loan = await loanService.FindByReservationId(reservation.Id);
                    if(loan != null)
                    {
                        reservation.LoanStatusId = (long)loan.Status; 
                    }
                }
                book.Reservations = reservationsDTO;
            }

            return Ok(booksDTO);
        }

        [HttpGet("isBookAvailable/{bookId:long}", Name = "IsBookAvailable")]
        public async Task<IActionResult> IsBookAvailable(long bookId)
        {
            var book = await bookService.FindById(bookId);
            if (book == null)
            {
                return NotFound($"Nenhuma livro encontrada com o ID: {bookId}.");
            }
            var isAvailable = await bookService.IsBookAvailable(book);
            return Ok(isAvailable);
        }
    }
}
