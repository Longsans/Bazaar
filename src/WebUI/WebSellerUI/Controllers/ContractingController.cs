using Microsoft.AspNetCore.Mvc;

namespace WebSellerUI.Controllers
{
    [ApiController]
    public class ContractingController : ControllerBase
    {
        private readonly ContractingService _contractingService;

        public ContractingController(ContractingService contractingService)
        {
            _contractingService = contractingService;
        }

        [HttpGet("/api/contracts")]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByPartnerId(
            [FromQuery] string partnerId)
        {
            if (string.IsNullOrWhiteSpace(partnerId))
            {
                return BadRequest("Partner ID must be specified.");
            }

            var contracts = await _contractingService.GetContractsByPartnerId(partnerId);
            if (contracts == null)
            {
                return Unauthorized();
            }
            return contracts.ToList();
        }

        [HttpGet("/api/partners/{partnerId}")]
        public async Task<ActionResult<Partner>> GetPartnerById(string partnerId)
        {
            if (string.IsNullOrWhiteSpace(partnerId))
            {
                return BadRequest("Partner ID must be specified.");
            }

            var partner = await _contractingService.GetPartnerById(partnerId);
            if (partner == null)
            {
                return Unauthorized();
            }
            return partner;
        }
    }
}
