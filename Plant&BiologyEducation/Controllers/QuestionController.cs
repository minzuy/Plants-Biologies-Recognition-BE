using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using System;
using System.Collections.Generic;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionRepository _questionRepo;
        private readonly IMapper _mapper;

        public QuestionController(QuestionRepository questionRepo, IMapper mapper)
        {
            _questionRepo = questionRepo;
            _mapper = mapper;
        }

        // GET: api/Question
        [HttpGet]
        public IActionResult GetAllQuestions()
        {
            var questions = _questionRepo.GetAllQuestions();
            var questionsDTO = _mapper.Map<List<QuestionDTO>>(questions);
            return Ok(questionsDTO);
        }

        // GET: api/Question/{id}
        [HttpGet("{id}")]
        public IActionResult GetQuestionById(int id)
        {
            if (!_questionRepo.QuestionExists(id))
                return NotFound("Không tìm thấy câu hỏi.");

            var question = _questionRepo.GetQuestionById(id);
            var questionDTO = _mapper.Map<QuestionDTO>(question);
            return Ok(questionDTO);
        }

        // POST: api/Question
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult CreateQuestion([FromBody] QuestionRequestDTO questionRequestDTO)
        {
            if (questionRequestDTO == null)
                return BadRequest("Dữ liệu câu hỏi là bắt buộc.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = _mapper.Map<Question>(questionRequestDTO);

            // KHÔNG gán question.Id nếu nó là auto-increment từ DB

            if (!_questionRepo.CreateQuestion(question))
                return StatusCode(500, "Có lỗi xảy ra khi tạo câu hỏi.");

            var questionDTO = _mapper.Map<QuestionDTO>(question);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, questionDTO);
        }


        // PUT: api/Question/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult UpdateQuestion(int id, [FromBody] QuestionRequestDTO questionRequestDTO)
        {
            if (questionRequestDTO == null)
                return BadRequest("Dữ liệu câu hỏi là bắt buộc.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_questionRepo.QuestionExists(id))
                return NotFound("Không tìm thấy câu hỏi.");

            var existingQuestion = _questionRepo.GetQuestionById(id);

            _mapper.Map(questionRequestDTO, existingQuestion);
            existingQuestion.Id = id;

            if (!_questionRepo.UpdateQuestion(existingQuestion))
                return StatusCode(500, "Có lỗi xảy ra khi cập nhật câu hỏi.");

            return NoContent();
        }

        // DELETE: api/Question/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult DeleteQuestion(int id)
        {
            if (!_questionRepo.QuestionExists(id))
                return NotFound("Không tìm thấy câu hỏi.");

            var question = _questionRepo.GetQuestionById(id);
            if (question == null)
                return NotFound();

            if (!_questionRepo.DeleteQuestion(question))
                return StatusCode(500, "Có lỗi xảy ra khi xóa câu hỏi.");

            return NoContent();
        }
    }
}
