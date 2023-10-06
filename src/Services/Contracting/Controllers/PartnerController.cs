namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PartnerController : ControllerBase
{
    private readonly PartnerManager _partnerManager;

    public PartnerController(PartnerManager partnerManager)
    {
        _partnerManager = partnerManager;
    }

    [HttpGet("{id}")]
    public ActionResult<PartnerQuery> GetById(int id)
    {
        var partner = _partnerManager.GetWithContractsById(id);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpGet]
    public ActionResult<PartnerQuery> GetByExternalId(string externalId)
    {
        var partner = _partnerManager.GetWithContractsByExternalId(externalId);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpPost]
    public ActionResult<PartnerQuery> Register(PartnerWriteCommand command)
    {
        var registerResult = _partnerManager.RegisterPartner(command.ToPartnerInfo());

        return registerResult switch
        {
            PartnerSuccessResult r => CreatedAtAction(
                nameof(GetById), new { r.Partner.Id }, new PartnerQuery(r.Partner)),
            PartnerUnderEighteenError => BadRequest(new { error = "Partner must be 18 or older." }),
            PartnerEmailAlreadyExistsError => Conflict(new { error = "This email already exists." }),
            _ => StatusCode(500)
        };
    }

    [HttpPut("{externalId}")]
    public ActionResult<PartnerQuery> UpdateInfo(string externalId, PartnerWriteCommand command)
    {
        var updatePartner = command.ToPartner(externalId);
        var updateResult = _partnerManager.UpdatePartnerInfoByExternalId(updatePartner);

        return updateResult switch
        {
            PartnerSuccessResult r => new PartnerQuery(r.Partner),
            PartnerUnderEighteenError => BadRequest(new { error = "Partner must be 18 or older." }),
            PartnerEmailAlreadyExistsError => Conflict(new { error = "This email already exists." }),
            PartnerNotFoundError => NotFound(new { externalId }),
            _ => StatusCode(500)
        };
    }
}