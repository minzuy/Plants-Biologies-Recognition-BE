using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.Chapter;
using Plant_BiologyEducation.Entity.DTO.P_B_A;
using Plant_BiologyEducation.Repository;
using PlantBiologyEducation.Entity.DTO.P_B_A;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BiologiesController : ControllerBase
    {
        private readonly Plant_Biology_Animal_Repository _pbaRepo;
        private readonly LessonRepository _lessonRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BiologiesController> _logger;

        public BiologiesController(
            Plant_Biology_Animal_Repository pbaRepo,
            LessonRepository lessonRepository,
            IMapper mapper,
            ILogger<BiologiesController> logger)
        {
            _pbaRepo = pbaRepo;
            _lessonRepository = lessonRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("search")]
        public IActionResult SearchByName([FromQuery] string? input)
        {
            var list = string.IsNullOrWhiteSpace(input)
                ? _pbaRepo.GetAllEntity()
                : _pbaRepo.SearchByName(input);

            var result = _mapper.Map<List<P_B_A_DTO>>(list);
            return Ok(result);
        }

        [HttpGet("pending")]
        public IActionResult GetPendingPBA()
        {
            var pendingList = _pbaRepo.GetPendingPBA();
            var dtoList = _mapper.Map<List<P_B_A_DTO>>(pendingList);
            return Ok(dtoList);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(Guid id, [FromBody] PBAStatusUpdateDTO statusDto)
        {
            var entity = _pbaRepo.GetById(id);
            if (entity == null)
            {
                _logger.LogWarning("Biology not found. Id: {Id}", id);
                return NotFound(new { error = "Biology not found", id });
            }

            var validStatuses = new[] { "Approved", "Rejected" };
            if (!validStatuses.Contains(statusDto.Status))
            {
                _logger.LogWarning("Invalid status for Biology update. Id: {Id}, Status: {Status}", id, statusDto.Status);
                return BadRequest(new { error = "Invalid status. Must be 'Approved' or 'Rejected'.", id, status = statusDto.Status });
            }

            entity.Status = statusDto.Status;

            if (statusDto.Status == "Rejected")
            {
                entity.RejectionReason = statusDto.RejectionReason ?? "No reason provided";
                entity.IsActive = false;
            }
            else
            {
                entity.RejectionReason = null;
                entity.IsActive = true;
            }

            if (!_pbaRepo.UpdatePBA(entity))
            {
                _logger.LogError("Failed to update biology status. Id: {Id}", id);
                return StatusCode(500, new { error = "Failed to update status", id });
            }

            return Ok(new
            {
                message = "Biology's status updated.",
                newStatus = entity.Status,
                pbaId = entity.Id
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var entity = _pbaRepo.GetById(id);
            if (entity == null)
            {
                _logger.LogWarning("Biology not found. Id: {Id}", id);
                return NotFound(new { error = "Biology not found", id });
            }

            var dto = _mapper.Map<P_B_A_DTO>(entity);
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] P_B_A_RequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                _logger.LogWarning("Request body is null.");
                return BadRequest(new { error = "Plant data is required." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState invalid: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var lesson = _lessonRepository.GetLessonById(requestDTO.Lesson_Id);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson not found for Lesson_Id: {LessonId}", requestDTO.Lesson_Id);
                return NotFound(new { error = "Lesson not found", lessonId = requestDTO.Lesson_Id });
            }

            var entity = _mapper.Map<Plant_Biology_Animals>(requestDTO);
            entity.Id = Guid.NewGuid();

            if (!_pbaRepo.CreatePBA(entity))
            {
                _logger.LogError("Failed to create biology object. {@Entity}", entity);
                return StatusCode(500, new { error = "Error while saving Object." });
            }

            var result = _mapper.Map<P_B_A_DTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetLessonByChapterId(Guid lessonId)
        {
            if (!_lessonRepository.LessonExists(lessonId))
            {
                _logger.LogWarning("Lesson not found. LessonId: {LessonId}", lessonId);
                return NotFound(new { error = "Lesson not found", lessonId });
            }

            var bios = await _pbaRepo.GetByLessonId(lessonId);
            var bioDTOs = _mapper.Map<List<P_B_A_DTO>>(bios);
            return Ok(bioDTOs);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] P_B_A_RequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                _logger.LogWarning("Update request is null.");
                return BadRequest(new { error = "Plant data is required." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state on update: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var lesson = _lessonRepository.GetLessonById(requestDTO.Lesson_Id);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson not found on update. LessonId: {LessonId}", requestDTO.Lesson_Id);
                return NotFound(new { error = "Lesson not found", lessonId = requestDTO.Lesson_Id });
            }

            if (!_pbaRepo.PBAExists(id))
            {
                _logger.LogWarning("Biology not found on update. Id: {Id}", id);
                return NotFound(new { error = "Biology not found", id });
            }

            var existing = _pbaRepo.GetById(id);
            _mapper.Map(requestDTO, existing);
            existing.Id = id;

            if (!_pbaRepo.UpdatePBA(existing))
            {
                _logger.LogError("Error while updating biology. Id: {Id}, Data: {@Data}", id, requestDTO);
                return StatusCode(500, new { error = "Error while updating plant.", id });
            }

            return Ok(new { message = "Updated successfully", id });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!_pbaRepo.PBAExists(id))
            {
                _logger.LogWarning("Biology not found on delete. Id: {Id}", id);
                return NotFound(new { error = "Biology not found", id });
            }

            var entity = _pbaRepo.GetById(id);
            if (entity == null)
            {
                _logger.LogWarning("Biology entity is null on delete. Id: {Id}", id);
                return NotFound(new { error = "Biology entity is null", id });
            }

            if (!_pbaRepo.DeletePBA(entity))
            {
                _logger.LogError("Error while deleting biology. Id: {Id}", id);
                return StatusCode(500, new { error = "Error while deleting plant.", id });
            }

            return Ok(new { message = "Deleted successfully", id });
        }
    }
}
