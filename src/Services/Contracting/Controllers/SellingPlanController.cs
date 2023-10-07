namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SellingPlanController : ControllerBase
{
    private readonly ISellingPlanManager _planManager;

    public SellingPlanController(ISellingPlanManager planManager)
    {
        _planManager = planManager;
    }

    [HttpGet("{id}")]
    public ActionResult<SellingPlan> GetById(int id)
    {
        var plan = _planManager.GetById(id);

        if (plan == null)
            return NotFound();

        return plan;
    }

    [HttpPost]
    public ActionResult<SellingPlan> CreatePlan(SellingPlanWriteCommand command)
    {
        var createResult = _planManager
            .CreateSellingPlan(command.PlanInfo());

        return createResult switch
        {
            SellingPlanSuccessResult r => r.Plan,
            PerSaleFeeAndMonthlyFeeNotPositiveError => BadRequest(
                new { error = "Either per sale fee or monthly fee has to be greater than 0." }),
            RegularPerSaleFeeNotPositiveError => BadRequest(
                new { error = "Regular per sale fee percentage has to be greater than 0." }),
            _ => StatusCode(500)
        };
    }

    [HttpPut("{id}")]
    public ActionResult<SellingPlan> UpdatePlan(int id, SellingPlanWriteCommand command)
    {
        var updateResult = _planManager
            .UpdateSellingPlan(command.ToSellingPlan(id));

        return updateResult switch
        {
            SellingPlanSuccessResult r => r.Plan,
            PerSaleFeeAndMonthlyFeeNotPositiveError => BadRequest(
                new { error = "Either per sale fee or monthly fee has to be greater than 0." }),
            RegularPerSaleFeeNotPositiveError => BadRequest(
                new { error = "Regular per sale fee percentage has to be greater than 0." }),
            SellingPlanNotFoundError => NotFound(new { error = "Selling plan not found." }),
            _ => StatusCode(500)
        };
    }
}
