using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.Chapter;
using Plant_BiologyEducation.Entity.DTO.P_B_A;
using Plant_BiologyEducation.Repository;
using PlantBiologyEducation.Entity.DTO.P_B_A;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BiologiesController : ControllerBase
    {
        private readonly Plant_Biology_Animal_Repository _pbaRepo;
        private readonly LessonRepository _lessonRepository;
        private readonly IMapper _mapper;

        public BiologiesController(
            Plant_Biology_Animal_Repository pbaRepo,
            LessonRepository lessonRepository,
            IMapper mapper)
        {
            _pbaRepo = pbaRepo;
            _lessonRepository = lessonRepository;
            _mapper = mapper;
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Student,Teacher")]

        public IActionResult SearchByName([FromQuery] string? input)
        {
            List<Plant_Biology_Animals> list;
            if (User.IsInRole("Student"))
            {
                list = (List<Plant_Biology_Animals>)(string.IsNullOrWhiteSpace(input)
                    ? _pbaRepo.GetApprovedPBA()
                    : _pbaRepo.SearchByNameForStudent(input));
            }
            else if (User.IsInRole("Teacher") || User.IsInRole("Admin"))
            {
                list = (List<Plant_Biology_Animals>)(string.IsNullOrWhiteSpace(input)
                    ? _pbaRepo.GetAllEntity()
                    : _pbaRepo.SearchByName(input));
            }
            else
            {
                return Forbid(); // Không hợp lệ
            }



            var result = _mapper.Map<List<P_B_A_DTO>>(list);
            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPendingPBA()
        {
            var pendingList = _pbaRepo.GetPendingPBA();
            var dtoList = _mapper.Map<List<P_B_A_DTO>>(pendingList);
            return Ok(dtoList);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(Guid id, [FromBody] PBAStatusUpdateDTO statusDto)
        {
            var entity = _pbaRepo.GetById(id);
            if (entity == null)
            {
                return NotFound(new { error = "Biology not found", id });
            }

            var validStatuses = new[] { "Approved", "Rejected" };
            if (!validStatuses.Contains(statusDto.Status))
            {
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
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(Guid id)
        {
            var entity = _pbaRepo.GetById(id);
            if (entity == null)
            {
                return NotFound(new { error = "Biology not found", id });
            }

            var dto = _mapper.Map<P_B_A_DTO>(entity);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create([FromBody] P_B_A_RequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                return BadRequest(new { error = "Plant data is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lesson = _lessonRepository.GetLessonById(requestDTO.Lesson_Id);
            if (lesson == null)
            {
                return NotFound(new { error = "Lesson not found", lessonId = requestDTO.Lesson_Id });
            }

            var entity = _mapper.Map<Plant_Biology_Animals>(requestDTO);
            entity.Id = Guid.NewGuid();
            entity.IsActive = false; // Default to active
            entity.Status = "Pending"; // Default status

            if (!_pbaRepo.CreatePBA(entity))
            {
                return StatusCode(500, new { error = "Error while saving Object." });
            }

            var result = _mapper.Map<P_B_A_DTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        [HttpGet("lesson/{lessonId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetLessonByChapterId(Guid lessonId)
        {
            if (!_lessonRepository.LessonExists(lessonId))
            {
                return NotFound(new { error = "Lesson not found", lessonId });
            }

            var bios = await _pbaRepo.GetByLessonId(lessonId);
            var bioDTOs = _mapper.Map<List<P_B_A_DTO>>(bios);
            return Ok(bioDTOs);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(Guid id, [FromBody] P_B_A_RequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                return BadRequest(new { error = "Plant data is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lesson = _lessonRepository.GetLessonById(requestDTO.Lesson_Id);
            if (lesson == null)
            {
                return NotFound(new { error = "Lesson not found", lessonId = requestDTO.Lesson_Id });
            }

            if (!_pbaRepo.PBAExists(id))
            {
                return NotFound(new { error = "Biology not found", id });
            }

            var existing = _pbaRepo.GetById(id);
            _mapper.Map(requestDTO, existing);
            existing.Id = id;
            existing.IsActive = false; // Ensure it's active on update
            existing.Status = "Pending"; // Reset status to pending on update

            if (!_pbaRepo.UpdatePBA(existing))
            {
                return StatusCode(500, new { error = "Error while updating plant.", id });
            }

            return Ok(new { message = "Updated successfully", id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public IActionResult Delete(Guid id)
        {
            if (!_pbaRepo.PBAExists(id))
            {
                return NotFound(new { error = "Biology not found", id });
            }

            var entity = _pbaRepo.GetById(id);
            if (entity == null)
            {
                return NotFound(new { error = "Biology entity is null", id });
            }

            if (!_pbaRepo.DeletePBA(entity))
            {
                return StatusCode(500, new { error = "Error while deleting plant.", id });
            }

            return Ok(new { message = "Deleted successfully", id });
        }
    }
}
