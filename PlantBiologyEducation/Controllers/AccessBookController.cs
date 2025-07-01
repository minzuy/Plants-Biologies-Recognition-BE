using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.DTO.AccessHistories;
using PlantBiologyEducation.Entity.Model;
using PlantBiologyEducation.Entity.DTO.AccessHistories;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessBookController : ControllerBase
    {
        private readonly AccessBookRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserRepository _user;
        private readonly BookRepository _book;
        public AccessBookController(AccessBookRepository repo, IMapper mapper,UserRepository user, BookRepository book)
        {
            _repo = repo;
            _mapper = mapper;
            _user = user;
            _book = book;
        }

        // POST: /api/AccessBook/record
        [HttpPost("record")]
        public IActionResult RecordAccess([FromBody] AccessBookRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_user.UserExists(dto.User_Id))
                return NotFound("User not existed");

            if (!_book.BookExists(dto.Book_Id))
                return NotFound("Book not existed");
            var success = _repo.RecordAccess(dto.User_Id, dto.Book_Id);
            if (!success)
                return StatusCode(500, "Failed to record access.");

            return Ok(new { message = "Access recorded successfully." });
        }

        // GET: /api/AccessBook/book/{bookId}/statistics
        [HttpGet("book/{bookId}/statistics")]
        public IActionResult GetBookStatistics(Guid bookId)
        {
            var uniqueVisitors = _repo.CountUniqueUsersForBook(bookId);
            var visitedNumber = _repo.TotalVisitedNumber(bookId);

            return Ok(new
            {
                bookId,
                uniqueVisitors,
                visitedNumber
            });
        }

    }
}
