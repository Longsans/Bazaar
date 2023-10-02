namespace Bazaar.Contracting.Controllers;

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
    public ActionResult<PartnerQuery> GetById(int id)
    {
        var partner = _partnerRepo.GetWithContractsById(id);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpGet]
    public ActionResult<PartnerQuery> GetByExternalId(string externalId)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(externalId);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpPost]
    public ActionResult<PartnerQuery> CreateNew(PartnerWriteCommand createCommand)
    {
        var created = _partnerRepo.Create(createCommand.ToPartnerInfo());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new PartnerQuery(created));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateInfo(int id, PartnerWriteCommand updateCommand)
    {
        var update = updateCommand.ToPartnerInfo();
        update.Id = id;

        if (!_partnerRepo.UpdateInfo(update))
            return NotFound();

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_partnerRepo.Delete(id))
            return NotFound(id);

        return Ok();
    }
}