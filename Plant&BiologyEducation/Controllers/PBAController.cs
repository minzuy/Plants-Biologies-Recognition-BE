using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Plant_BiologyEducation.Entity.DTO.P_B_A;
using Plant_BiologyEducation.Repository;

namespace Plant_BiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PBAController : ControllerBase
    {
        private readonly Plant_Biology_Animal_Repository _pbaRepo;
        private readonly IMapper _mapper;

        public PBAController(Plant_Biology_Animal_Repository pbaRepo, IMapper mapper)
        {
            _pbaRepo = pbaRepo;
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

        // GET: api/PBA/{id}
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
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
