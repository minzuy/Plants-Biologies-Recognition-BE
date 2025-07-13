using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Plant_BiologyEducation.Entity.DTO.Book;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using PlantBiologyEducation.Entity.DTO.Book;
using Plant_BiologyEducation.Entity.DTO.User;

namespace Plant_BiologyEducation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookController(BookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet("approved")]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _bookRepository.GetAllBooksForStudents();
                var booksDTO = _mapper.Map<List<BookDTO>>(books);
                return Ok(booksDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchOrGetAllBooks([FromQuery] string? title)
        {
            try
            {
                var books = string.IsNullOrWhiteSpace(title)
                    ? _bookRepository.GetAllBooks()
                    : _bookRepository.SearchBooksByTitle(title);

                foreach (var book in books)
                {
                    book.Chapters = book.Chapters
                        .OrderBy(c => c.Chapter_Title)
                        .Select(c =>
                        {
                            c.Lessons = c.Lessons
                                .OrderBy(l => l.Lesson_Title)
                                .ToList();
                            return c;
                        })
                        .ToList();
                }

                var bookDTOs = _mapper.Map<List<BookDTO>>(books);
                return Ok(bookDTOs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public IActionResult GetPendingBooks()
        {
            try
            {
                var books = _bookRepository.GetPendingBooks();
                var booksDTO = _mapper.Map<List<BookDTO>>(books);
                return Ok(booksDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBookById(Guid id)
        {
            try
            {
                var book = _bookRepository.GetBookById(id);
                if (book == null)
                    return NotFound();

                var bookDTO = _mapper.Map<BookDTO>(book);
                return Ok(bookDTO);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateBookWithChaptersAndLessons([FromBody] BookRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var book = _mapper.Map<Book>(dto);
                book.Book_Id = Guid.NewGuid();
                book.Status = "Pending";
                book.IsActive = false;
                book.RejectionReason = null;

                //foreach (var chapter in book.Chapters)
                //{
                //    chapter.Chapter_Id = Guid.NewGuid();
                //    chapter.Book_Id = book.Book_Id;
                //    chapter.Status = "Pending";
                //    chapter.IsActive = false;
                //    chapter.RejectionReason = null;

                //    foreach (var lesson in chapter.Lessons)
                //    {
                //        lesson.Lesson_Id = Guid.NewGuid();
                //        lesson.Chapter_Id = chapter.Chapter_Id;
                //        lesson.Status = "Pending";
                //        lesson.IsActive = false;
                //        lesson.RejectionReason = null;
                //    }
                //}

                var success = _bookRepository.CreateBook(book);
                if (!success)
                    return StatusCode(500, "An error occurred while saving the book.");

                return Ok(new { message = "Book created successfully", bookId = book.Book_Id });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(Guid id, [FromBody] BookRequestDTO bookDto)
        {
            try
            {
                if (!_bookRepository.BookExists(id))
                    return NotFound("Book not found.");

                var bookEntity = _mapper.Map<Book>(bookDto);
                bookEntity.Book_Id = id;
                bookEntity.Status = "Pending";
                bookEntity.IsActive = false;

                var result = _bookRepository.UpdateBook(bookEntity);
                if (!result)
                    return StatusCode(500, "Error updating the book.");

                return Ok("Book updated successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult ApproveOrRejectBook(Guid id, [FromBody] BookStatusUpdateDTO statusDto)
        {
            try
            {
                var book = _bookRepository.GetBookById(id);
                if (book == null)
                    return NotFound("Book not found.");

                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(statusDto.Status))
                    return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");

                book.Status = statusDto.Status;

                if (statusDto.Status == "Rejected")
                {
                    book.RejectionReason = statusDto.RejectionReason ?? "No reason provided";
                    book.IsActive = false;
                }
                else if (statusDto.Status == "Approved")
                {
                    book.RejectionReason = null;
                    book.IsActive = true;
                }

                var result = _bookRepository.UpdateBook(book);
                if (!result)
                    return StatusCode(500, "Failed to update status.");

                return Ok(new
                {
                    message = "Book status updated.",
                    newStatus = book.Status,
                    bookId = book.Book_Id
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(Guid id)
        {
            try
            {
                var book = _bookRepository.GetBookById(id);
                if (book == null)
                    return NotFound();

                var result = _bookRepository.DeleteBook(book);
                if (!result)
                    return StatusCode(500, "Error deleting the book.");

                return Ok("Book deleted successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
