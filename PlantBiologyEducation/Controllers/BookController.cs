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
        private readonly ILogger<BookController> _logger;

        public BookController(BookRepository bookRepository, IMapper mapper, ILogger<BookController> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("approved")]
        public IActionResult GetAllBooks()
        {
            try
            {
                _logger.LogInformation("GET /api/Book/approved called from Mobile");
                var books = _bookRepository.GetAllBooksForStudents();
                var booksDTO = _mapper.Map<List<BookDTO>>(books);
                return Ok(booksDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching approved books.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchOrGetAllBooks([FromQuery] string? title)
        {
            try
            {
                _logger.LogInformation("GET /api/Book/search?title={title} called", title);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching books.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public IActionResult GetPendingBooks()
        {
            try
            {
                _logger.LogInformation("GET /api/Book/pending called");
                var books = _bookRepository.GetPendingBooks();
                var booksDTO = _mapper.Map<List<BookDTO>>(books);
                return Ok(booksDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching pending books.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBookById(Guid id)
        {
            try
            {
                _logger.LogInformation("GET /api/Book/{id} called with id: {Id}", id);
                var book = _bookRepository.GetBookById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book not found with id: {Id}", id);
                    return NotFound();
                }

                var bookDTO = _mapper.Map<BookDTO>(book);
                return Ok(bookDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting book by id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateBookWithChaptersAndLessons([FromBody] BookRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("POST /api/Book called to create new book.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var book = _mapper.Map<Book>(dto);
                book.Book_Id = Guid.NewGuid();
                book.Status = "Pending";
                book.IsActive = false;
                book.RejectionReason = null;

                foreach (var chapter in book.Chapters)
                {
                    chapter.Chapter_Id = Guid.NewGuid();
                    chapter.Book_Id = book.Book_Id;
                    chapter.Status = "Pending";
                    chapter.IsActive = false;
                    chapter.RejectionReason = null;

                    foreach (var lesson in chapter.Lessons)
                    {
                        lesson.Lesson_Id = Guid.NewGuid();
                        lesson.Chapter_Id = chapter.Chapter_Id;
                        lesson.Status = "Pending";
                        lesson.IsActive = false;
                        lesson.RejectionReason = null;
                    }
                }

                var success = _bookRepository.CreateBook(book);
                if (!success)
                {
                    _logger.LogError("Error saving book to database.");
                    return StatusCode(500, "An error occurred while saving the book.");
                }

                _logger.LogInformation("Book created successfully with ID: {BookId}", book.Book_Id);
                return Ok(new { message = "Book created successfully", bookId = book.Book_Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating book.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(Guid id, [FromBody] BookRequestDTO bookDto)
        {
            try
            {
                _logger.LogInformation("PUT /api/Book/{id} called to update book: {Id}", id);

                if (!_bookRepository.BookExists(id))
                {
                    _logger.LogWarning("Book not found with id: {Id}", id);
                    return NotFound("Book not found.");
                }

                var bookEntity = _mapper.Map<Book>(bookDto);
                bookEntity.Book_Id = id;
                bookEntity.Status = "Pending";
                bookEntity.IsActive = false;

                var result = _bookRepository.UpdateBook(bookEntity);
                if (!result)
                {
                    _logger.LogError("Error updating book with id: {Id}", id);
                    return StatusCode(500, "Error updating the book.");
                }

                _logger.LogInformation("Book updated successfully with id: {Id}", id);
                return Ok("Book updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating book with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult ApproveOrRejectBook(Guid id, [FromBody] BookStatusUpdateDTO statusDto)
        {
            try
            {
                _logger.LogInformation("PUT /api/Book/{id}/status called with id: {Id} and status: {Status}", id, statusDto.Status);

                var book = _bookRepository.GetBookById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book not found with id: {Id}", id);
                    return NotFound("Book not found.");
                }

                var validStatuses = new[] { "Approved", "Rejected" };
                if (!validStatuses.Contains(statusDto.Status))
                {
                    _logger.LogWarning("Invalid status: {Status}", statusDto.Status);
                    return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");
                }

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
                {
                    _logger.LogError("Failed to update book status with id: {Id}", id);
                    return StatusCode(500, "Failed to update status.");
                }

                _logger.LogInformation("Book status updated to {Status} for book id: {Id}", statusDto.Status, id);
                return Ok(new
                {
                    message = "Book status updated.",
                    newStatus = book.Status,
                    bookId = book.Book_Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while updating status of book with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(Guid id)
        {
            try
            {
                _logger.LogInformation("DELETE /api/Book/{id} called to delete book with id: {Id}", id);
                var book = _bookRepository.GetBookById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book not found for deletion with id: {Id}", id);
                    return NotFound();
                }

                var result = _bookRepository.DeleteBook(book);
                if (!result)
                {
                    _logger.LogError("Error deleting book with id: {Id}", id);
                    return StatusCode(500, "Error deleting the book.");
                }

                _logger.LogInformation("Book deleted successfully with id: {Id}", id);
                return Ok("Book deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting book with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
