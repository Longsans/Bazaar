namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PartnerController : ControllerBase
{
    private readonly IPartnerUsecases _partnerUsecases;

    public PartnerController(IPartnerUsecases partnerUsecases)
    {
        _partnerUsecases = partnerUsecases;
    }

    [HttpGet("{id}")]
    public ActionResult<PartnerQuery> GetById(int id)
    {
        var partner = _partnerUsecases.GetWithContractsById(id);
        if (partner == null)
            return NotFound();

        return new PartnerQuery(partner);
    }

    [HttpGet]
    public ActionResult<PartnerQuery> GetByExternalId(string externalId)
    {
        var partner = _partnerUsecases.GetWithContractsByExternalId(externalId);
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
        var registerResult = _partnerUsecases.RegisterPartner(partnerDto);

        //return registerResult.Status switch
        //{
        //    ResultStatus.Ok => CreatedAtAction(
        //        nameof(GetById), new { registerResult.Value.Id }, registerResult.Value),
        //    ResultStatus.Invalid => BadRequest(
        //        new { errors = registerResult.ValidationErrors.Select(e => e.ErrorMessage) }),
        //    ResultStatus.Conflict => Conflict(new { errors = registerResult.Errors }),
        //    _ => StatusCode(500)
        //};

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

        var updateResult = _partnerUsecases.UpdatePartnerInfoByExternalId(partnerUpdateDto);

        return updateResult.ToActionResult(this);
    }
}