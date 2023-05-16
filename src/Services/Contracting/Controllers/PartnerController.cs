using Microsoft.AspNetCore.Mvc;
using Contracting.Model;
using Contracting.Dto;

namespace Contracting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerRepository _partnerRepo;

        public PartnerController(IPartnerRepository partnerRepo)
        {
            _partnerRepo = partnerRepo;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var partner = _partnerRepo.GetById(id);
            if (partner == null)
                return NotFound();
            return Ok(new PartnerQuery(partner));
        }

        [HttpGet("ext/{externalId}")]
        public IActionResult GetByExternalId(string externalId)
        {
            var partner = _partnerRepo.GetByExternalId(externalId);
            if (partner == null)
                return NotFound();
            return Ok(new PartnerQuery(partner));
        }
    }
}