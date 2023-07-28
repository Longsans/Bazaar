namespace Bazaar.Contracting.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerRepository _partnerRepo;

        public PartnerController(IPartnerRepository partnerRepo)
        {
            _partnerRepo = partnerRepo;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id, [FromQuery] string? externalId = null)
        {
            Partner? partner;
            if (externalId != null)
            {
                partner = _partnerRepo.GetByExternalId(externalId);
            }
            else
            {
                partner = _partnerRepo.GetById(id);
            }
            if (partner == null)
            {
                return NotFound();
            }
            return Ok(new PartnerQuery(partner));
        }

        [HttpPost]
        public ActionResult<Partner> Create(Partner partner)
        {
            var created = _partnerRepo.Create(partner);
            return CreatedAtAction(nameof(GetById), created.Id, created);
        }

        [HttpPut]
        public IActionResult Update(Partner partner)
        {
            if (_partnerRepo.Update(partner))
            {
                return Ok();
            }
            return NotFound(partner.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_partnerRepo.Delete(id))
            {
                return Ok();
            }
            return NotFound(id);
        }
    }
}