using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Entity.DTO;
using AutoMapper;

namespace Plant_BiologyEducation.Controllers
{
    public class QuestionController : Controller
    {
        private readonly QuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public QuestionController(QuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        // READ - GET: Question (Index - Hiển thị danh sách tất cả câu hỏi)
        [HttpGet]
        public IActionResult Index()
        {
            var questions = _questionRepository.GetAllQuestions();
            return View(questions);
        }

        // READ - GET: Question/Details/5 (Xem chi tiết một câu hỏi)
        [HttpGet]
        public IActionResult Details(int id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // CREATE - GET: Question/Create (Hiển thị form tạo câu hỏi)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST: Question/Create (Xử lý tạo câu hỏi mới)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuestionDTO questionDto)
        {
            if (ModelState.IsValid)
            {
                // Sử dụng AutoMapper để chuyển đổi từ DTO sang Entity
                var question = _mapper.Map<Question>(questionDto);
                // Id sẽ được tự động sinh bởi database (Identity/Auto-increment)
                question.Id = 0; // Đảm bảo Id = 0 để database tự sinh

                if (_questionRepository.CreateQuestion(question))
                {
                    TempData["Success"] = "Câu hỏi đã được tạo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi tạo câu hỏi!";
                }
            }
            return View(questionDto);
        }

        // UPDATE - GET: Question/Edit/5 (Hiển thị form chỉnh sửa câu hỏi)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }

            // Sử dụng AutoMapper để chuyển đổi từ Entity sang DTO
            var questionDto = _mapper.Map<QuestionDTO>(question);
            return View(questionDto);
        }

        // UPDATE - POST: Question/Edit/5 (Xử lý cập nhật câu hỏi)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, QuestionDTO questionDto)
        {
            if (id != questionDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Sử dụng AutoMapper để chuyển đổi từ DTO sang Entity
                var question = _mapper.Map<Question>(questionDto);

                if (_questionRepository.UpdateQuestion(question))
                {
                    TempData["Success"] = "Câu hỏi đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật câu hỏi!";
                }
            }
            return View(questionDto);
        }

        // DELETE - GET: Question/Delete/5 (Hiển thị form xác nhận xóa)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // DELETE - POST: Question/Delete/5 (Xử lý xóa câu hỏi)
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question != null)
            {
                if (_questionRepository.DeleteQuestion(question))
                {
                    TempData["Success"] = "Câu hỏi đã được xóa thành công!";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa câu hỏi!";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //// ADDITIONAL - GET: Question/GetByTest/5 (Lấy câu hỏi theo TestId)
        //[HttpGet]
        //public IActionResult GetByTest(int testId)
        //{
        //    var questions = _questionRepository.GetQuestionsByTestId(testId);
        //    return View("Index", questions);
        //}

        //// API ENDPOINT - GET: Question/GetQuestionsByTestId (API cho AJAX calls)
        //[HttpGet]
        //public JsonResult GetQuestionsByTestId(int testId)
        //{
        //    var questions = _questionRepository.GetQuestionsByTestId(testId);
        //    // Sử dụng AutoMapper để chuyển đổi từ Entity sang DTO
        //    var questionDtos = _mapper.Map<List<QuestionDTO>>(questions);
        //    return Json(questionDtos);
        //}

        // API ENDPOINTS - RESTful API cho Question CRUD

        // API CREATE - POST: api/Question
        [HttpPost]
        [Route("api/[controller]")]
        public IActionResult CreateQuestion([FromBody] QuestionDTO questionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = _mapper.Map<Question>(questionDto);
            question.Id = 0; // Database tự sinh ID

            if (_questionRepository.CreateQuestion(question))
            {
                var createdQuestionDto = _mapper.Map<QuestionDTO>(question);
                return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, createdQuestionDto);
            }

            return StatusCode(500, "Có lỗi xảy ra khi tạo câu hỏi");
        }

        // API READ - GET: api/Question/5
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IActionResult GetQuestion(int id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }

            var questionDto = _mapper.Map<QuestionDTO>(question);
            return Ok(questionDto);
        }

        // API READ ALL - GET: api/Question
        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetAllQuestions()
        {
            var questions = _questionRepository.GetAllQuestions();
            var questionDtos = _mapper.Map<List<QuestionDTO>>(questions);
            return Ok(questionDtos);
        }

        // API UPDATE - PUT: api/Question/5
        [HttpPut]
        [Route("api/[controller]/{id}")]
        public IActionResult UpdateQuestion(int id, [FromBody] QuestionDTO questionDto)
        {
            if (id != questionDto.Id)
            {
                return BadRequest("ID không khớp");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_questionRepository.QuestionExists(id))
            {
                return NotFound();
            }

            var question = _mapper.Map<Question>(questionDto);

            if (_questionRepository.UpdateQuestion(question))
            {
                return NoContent(); // 204 No Content - cập nhật thành công
            }

            return StatusCode(500, "Có lỗi xảy ra khi cập nhật câu hỏi");
        }

        // API DELETE - DELETE: api/Question/5
        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public IActionResult DeleteQuestion(int id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }

            if (_questionRepository.DeleteQuestion(question))
            {
                return NoContent(); // 204 No Content - xóa thành công
            }

            return StatusCode(500, "Có lỗi xảy ra khi xóa câu hỏi");
        }
    }
}