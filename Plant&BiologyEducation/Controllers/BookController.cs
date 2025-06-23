using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Plant_BiologyEducation.Entity.DTO.Book;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using System.Xml;

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

        // GET: api/Book?title=abc
        [HttpGet("search")]
        public IActionResult SearchOrGetAllBooks([FromQuery] string? title)
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

        // GET: api/Book/{id}
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public IActionResult GetBookById(Guid id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
                return NotFound();

            var bookDTO = _mapper.Map<BookDTO>(book);
            return Ok(bookDTO);
        }

        // POST: api/Book
        [HttpPost]
        public IActionResult CreateBookWithChaptersAndLessons([FromBody] BookRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = _mapper.Map<Book>(dto);
            book.Book_Id = Guid.NewGuid();

            foreach (var chapter in book.Chapters)
            {
                chapter.Chapter_Id = Guid.NewGuid();
                chapter.Book_Id = book.Book_Id;

                foreach (var lesson in chapter.Lessons)
                {
                    lesson.Lesson_Id = Guid.NewGuid();
                    lesson.Chapter_Id = chapter.Chapter_Id;
                }
            }

            var success = _bookRepository.CreateBook(book);
            if (!success)
                return StatusCode(500, "An error occurred while saving the book.");

            return Ok(new { message = "Book created successfully", bookId = book.Book_Id });
        }


        // PUT: api/Book/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateBook(Guid id, [FromBody] BookRequestDTO bookDto)
        {
            // Kiểm tra sách có tồn tại
            if (!_bookRepository.BookExists(id))
                return NotFound("Book not found.");

            // Ánh xạ DTO sang Entity
            var bookEntity = _mapper.Map<Book>(bookDto);

            // Gán ID từ route vào entity
            bookEntity.Book_Id = id;

            // Thực hiện cập nhật
            var result = _bookRepository.UpdateBook(bookEntity);
            if (!result)
                return StatusCode(500, "Error updating the book.");

            return Ok("Book updated successfully.");
        }



        // DELETE: api/Book/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(Guid id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
                return NotFound();

            var result = _bookRepository.DeleteBook(book);
            if (!result)
                return StatusCode(500, "Error deleting the book.");

            return Ok("Book deleted successfully.");
        }
    }
}
