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
        private readonly TestRepository _testRepository;
        private readonly IMapper _mapper;

        public QuestionController(QuestionRepository questionRepository, TestRepository testRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _testRepository = testRepository;
            _mapper = mapper;
        }

        // MVC ACTIONS

        // READ - GET: Question (Index - Hiển thị danh sách tất cả câu hỏi)
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var questions = _questionRepository.GetAllQuestions();
                return View(questions);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải danh sách câu hỏi: {ex.Message}";
                return View(new List<Question>());
            }
        }

        // READ - GET: Question/Details/5 (Xem chi tiết một câu hỏi)
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID câu hỏi không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (!_questionRepository.QuestionExists(id))
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                var question = _questionRepository.GetQuestionById(id);
                if (question == null)
                {
                    return NotFound();
                }
                return View(question);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải chi tiết câu hỏi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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
        public IActionResult Create(QuestionRequestDTO questionRequestDTO)
        {
            try
            {
                if (questionRequestDTO == null)
                {
                    TempData["Error"] = "Dữ liệu câu hỏi là bắt buộc.";
                    return View();
                }

                if (!ModelState.IsValid)
                {
                    return View(questionRequestDTO);
                }

                // Kiểm tra TestId có tồn tại không
                if (!_testRepository.TestExists(questionRequestDTO.TestId.ToString()))
                {
                    ModelState.AddModelError("TestId", "Test ID không tồn tại.");
                    return View(questionRequestDTO);
                }

                // Map từ RequestDTO sang Entity
                var question = _mapper.Map<Question>(questionRequestDTO);
                question.Id = 0; // Database sẽ tự động sinh ID

                if (!_questionRepository.CreateQuestion(question))
                {
                    TempData["Error"] = "Có lỗi xảy ra khi lưu câu hỏi.";
                    return View(questionRequestDTO);
                }

                TempData["Success"] = "Câu hỏi đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(questionRequestDTO);
            }
        }

        // UPDATE - GET: Question/Edit/5 (Hiển thị form chỉnh sửa câu hỏi)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID câu hỏi không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (!_questionRepository.QuestionExists(id))
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                var question = _questionRepository.GetQuestionById(id);
                if (question == null)
                {
                    return NotFound();
                }

                // Map từ Entity sang RequestDTO để hiển thị form
                var questionRequestDTO = _mapper.Map<QuestionRequestDTO>(question);
                return View(questionRequestDTO);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải form chỉnh sửa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // UPDATE - POST: Question/Edit/5 (Xử lý cập nhật câu hỏi)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, QuestionRequestDTO questionRequestDTO)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID câu hỏi không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (questionRequestDTO == null)
                {
                    TempData["Error"] = "Dữ liệu câu hỏi là bắt buộc.";
                    return View();
                }

                if (!ModelState.IsValid)
                {
                    return View(questionRequestDTO);
                }

                if (!_questionRepository.QuestionExists(id))
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra TestId có tồn tại không
                if (!_testRepository.TestExists(questionRequestDTO.TestId.ToString()))
                {
                    ModelState.AddModelError("TestId", "Test ID không tồn tại.");
                    return View(questionRequestDTO);
                }

                // Lấy entity hiện tại từ database
                var existingQuestion = _questionRepository.GetQuestionById(id);
                if (existingQuestion == null)
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                // Map các thay đổi từ RequestDTO sang Entity (Id sẽ không bị thay đổi)
                _mapper.Map(questionRequestDTO, existingQuestion);
                existingQuestion.Id = id; // Đảm bảo ID không bị thay đổi

                if (!_questionRepository.UpdateQuestion(existingQuestion))
                {
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật câu hỏi.";
                    return View(questionRequestDTO);
                }

                TempData["Success"] = "Câu hỏi đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(questionRequestDTO);
            }
        }

        // DELETE - GET: Question/Delete/5 (Hiển thị form xác nhận xóa)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID câu hỏi không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (!_questionRepository.QuestionExists(id))
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                var question = _questionRepository.GetQuestionById(id);
                if (question == null)
                {
                    return NotFound();
                }
                return View(question);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải form xóa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // DELETE - POST: Question/Delete/5 (Xử lý xóa câu hỏi)
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID câu hỏi không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                if (!_questionRepository.QuestionExists(id))
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                var question = _questionRepository.GetQuestionById(id);
                if (question == null)
                {
                    TempData["Error"] = $"Không tìm thấy câu hỏi với ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                if (!_questionRepository.DeleteQuestion(question))
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa câu hỏi.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Câu hỏi đã được xóa thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // API ENDPOINTS - Kết hợp trong cùng controller như TestController

        // GET: api/Question
        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetAll()
        {
            try
            {
                var questions = _questionRepository.GetAllQuestions();
                var result = _mapper.Map<IEnumerable<QuestionDTO>>(questions);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Question/{id}
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Question ID must be greater than 0.");

                if (!_questionRepository.QuestionExists(id))
                    return NotFound($"Question with ID {id} not found.");

                var question = _questionRepository.GetQuestionById(id);
                var result = _mapper.Map<QuestionDTO>(question);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Question/test/{testId}
        [HttpGet]
        [Route("api/[controller]/test/{testId}")]
        public IActionResult GetByTestId(string testId)
        {
            try
            {
                if (!_testRepository.TestExists(testId.ToString()))
                    return NotFound($"Test with ID {testId} not found.");

                var questions = _questionRepository.GetQuestionsByTestId(testId);
                var result = _mapper.Map<IEnumerable<QuestionDTO>>(questions);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Question
        [HttpPost]
        [Route("api/[controller]")]
        public IActionResult CreateQuestion([FromBody] QuestionRequestDTO questionRequestDTO)
        {
            try
            {
                if (questionRequestDTO == null)
                    return BadRequest("Question data is required.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Kiểm tra TestId có tồn tại không
                if (!_testRepository.TestExists(questionRequestDTO.TestId.ToString()))
                {
                    return BadRequest("Invalid TestId. Test does not exist.");
                }

                // Map từ RequestDTO sang Entity
                var question = _mapper.Map<Question>(questionRequestDTO);
                question.Id = 0; // Database sẽ tự động sinh ID

                if (!_questionRepository.CreateQuestion(question))
                {
                    return StatusCode(500, "Something went wrong while saving question.");
                }

                // Reload question sau khi tạo để lấy đầy đủ thông tin
                var createdQuestion = _questionRepository.GetQuestionById(question.Id);
                if (createdQuestion == null)
                {
                    return StatusCode(500, "Failed to retrieve created question.");
                }

                var result = _mapper.Map<QuestionDTO>(createdQuestion);
                return CreatedAtAction(nameof(GetById), new { id = question.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }

        // PUT: api/Question/{id}
        [HttpPut]
        [Route("api/[controller]/{id}")]
        public IActionResult UpdateQuestion(int id, [FromBody] QuestionRequestDTO questionRequestDTO)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Question ID must be greater than 0.");

                if (questionRequestDTO == null)
                    return BadRequest("Question data is required.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_questionRepository.QuestionExists(id))
                    return NotFound($"Question with ID {id} not found.");

                // Kiểm tra TestId có tồn tại không
                if (!_testRepository.TestExists(questionRequestDTO.TestId.ToString()))
                {
                    return BadRequest("Invalid TestId. Test does not exist.");
                }

                // Lấy question hiện tại từ database
                var existingQuestion = _questionRepository.GetQuestionById(id);
                if (existingQuestion == null)
                    return NotFound($"Question with ID {id} not found.");

                // Map các thay đổi từ RequestDTO sang Entity (Id sẽ không bị thay đổi)
                _mapper.Map(questionRequestDTO, existingQuestion);
                existingQuestion.Id = id; // Đảm bảo ID không bị thay đổi

                if (!_questionRepository.UpdateQuestion(existingQuestion))
                {
                    return StatusCode(500, "Something went wrong while updating question.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }

        // DELETE: api/Question/{id}
        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public IActionResult DeleteQuestion(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Question ID must be greater than 0.");

                if (!_questionRepository.QuestionExists(id))
                    return NotFound($"Question with ID {id} not found.");

                var question = _questionRepository.GetQuestionById(id);
                if (question == null)
                    return NotFound($"Question with ID {id} not found.");

                if (!_questionRepository.DeleteQuestion(question))
                {
                    return StatusCode(500, "Something went wrong while deleting question.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }
    }
}