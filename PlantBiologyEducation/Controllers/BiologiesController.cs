using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        public BiologiesController(Plant_Biology_Animal_Repository pbaRepo,LessonRepository lessonRepository, IMapper mapper)
        {
            _pbaRepo = pbaRepo;
            _lessonRepository = lessonRepository;
            _mapper = mapper;
        }

        // GET: api/PBA/search?commonName=...
        [HttpGet("search")]
        public IActionResult SearchByCommonName([FromQuery] string? commonName)
        {
            var list = string.IsNullOrWhiteSpace(commonName)
                ? _pbaRepo.GetAllEntity()
                : _pbaRepo.SearchByCommonName(commonName);

            var result = _mapper.Map<List<P_B_A_DTO>>(list);
            return Ok(result);
        }
        // GET: api/biologies/pending
        [HttpGet("pending")]
        public IActionResult GetPendingPBA()
        {
            var pendingList = _pbaRepo.GetPendingPBA();
            var dtoList = _mapper.Map<List<P_B_A_DTO>>(pendingList);
            return Ok(dtoList);
        }

        // PUT: api/biologies/{id}/status
        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(Guid id, [FromBody] PBAStatusUpdateDTO statusDto)
        {
            var entity = _pbaRepo.GetById(id);
            if (entity == null)
                return NotFound("Bilogy not found");

            var validStatuses = new[] { "Approved", "Rejected" };
            if (!validStatuses.Contains(statusDto.Status))
                return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");

            entity.Status = statusDto.Status;

            if (statusDto.Status == "Rejected")
            {
                entity.RejectionReason = statusDto.RejectionReason ?? "No reason provided";
                entity.IsActive = false;
            }
            else if (statusDto.Status == "Approved")
            {
                entity.RejectionReason = null;
                entity.IsActive = true;
            }

            if (!_pbaRepo.UpdatePBA(entity))
                return StatusCode(500, "Failed to update status.");

            return Ok(new
            {
                message = "Bilogy's status updated.",
                newStatus = entity.Status,
                pbaId = entity.Id
            });
        }

        // GET: api/PBA/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var entity = _pbaRepo.GetById(id);
            if (entity == null)
                return NotFound();

            var dto = _mapper.Map<P_B_A_DTO>(entity);
            return Ok(dto);
        }

        // POST: api/PBA
        [HttpPost]
        public IActionResult Create([FromBody] P_B_A_RequestDTO requestDTO)
        {
            if (requestDTO == null)
                return BadRequest("Plant data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var lesson = _lessonRepository.GetLessonById(requestDTO.Lesson_Id);
            if (lesson == null)
                return NotFound("Lesson not found.");

            var entity = _mapper.Map<Plant_Biology_Animals>(requestDTO);
            entity.Id = Guid.NewGuid();

            if (!_pbaRepo.CreatePBA(entity))
                return StatusCode(500, "Error while saving plant.");

            var result = _mapper.Map<P_B_A_DTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        // PUT: api/PBA/{id}
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] P_B_A_RequestDTO requestDTO)
        {
            var lesson   = _lessonRepository.GetLessonById(requestDTO.Lesson_Id);
            if (lesson == null)
                return NotFound("Lesson not found.");
            if (requestDTO == null)
                return BadRequest("Plant data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_pbaRepo.PBAExists(id))
                return NotFound();

            var existing = _pbaRepo.GetById(id);
            _mapper.Map(requestDTO, existing);
            existing.Id = id;

            if (!_pbaRepo.UpdatePBA(existing))
                return StatusCode(500, "Error while updating plant.");

            return NoContent();
        }

        // DELETE: api/PBA/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!_pbaRepo.PBAExists(id))
                return NotFound();

            var entity = _pbaRepo.GetById(id);
            if (entity == null)
                return NotFound();

            if (!_pbaRepo.DeletePBA(entity))
                return StatusCode(500, "Error while deleting plant.");

            return NoContent();
        }
    }
}
