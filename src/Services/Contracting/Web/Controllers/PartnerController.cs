namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PartnerController : ControllerBase
{
    private readonly IPartnerUseCases _partnerUseCases;

    public PartnerController(IPartnerUseCases partnerUseCases)
    {
        _partnerUseCases = partnerUseCases;
    }

    [HttpGet("{id}")]
    public ActionResult<PartnerQuery> GetById(int id)
    {
        var partner = _partnerUseCases.GetWithContractsById(id);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpGet]
    public ActionResult<PartnerQuery> GetByExternalId(string externalId)
    {
        var partner = _partnerUseCases.GetWithContractsByExternalId(externalId);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpPost]
    public ActionResult<PartnerDto> Register(PartnerWriteRequest command)
    {
        var partnerDto = new PartnerDto()
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            Gender = command.Gender
        };

        var registerResult = _partnerUseCases.RegisterPartner(partnerDto);
        return registerResult.ToActionResult(this);
    }

    [HttpPut("{externalId}")]
    public IActionResult UpdateInfo(string externalId, PartnerWriteRequest command)
    {
        var partnerUpdateDto = new PartnerDto()
        {
            ExternalId = externalId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            Gender = command.Gender
        };

        var updateResult = _partnerUseCases
            .UpdatePartnerInfoByExternalId(partnerUpdateDto);

        return updateResult.ToActionResult(this);
    }
}