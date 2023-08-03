using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Catalog.Controllers
{
    [Route("api/coordinator")]
    [ApiController]
    public class TransactionCoordinatorController : ControllerBase
    {
        private readonly TransactionCoordinator _coordinator;
        private readonly ILogger<TransactionCoordinatorController> _logger;

        public TransactionCoordinatorController(TransactionCoordinator coordinator, ILogger<TransactionCoordinatorController> logger)
        {
            _coordinator = coordinator;
            _logger = logger;
        }

        [HttpPost("transactions")]
        public IActionResult BeginTransaction([FromBody] TransactionRef txn)
        {
            _coordinator.BeginTransaction(txn);
            return Ok();
        }

        [HttpPost("transactions/{txn}/indexes")]
        public IActionResult AddIndexToTransaction([FromRoute] TransactionRef txn, [FromBody] string index)
        {
            try
            {
                _coordinator.AddIndexToTransaction(txn, index);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("transactions/{txn}/commit")]
        public async Task<IActionResult> CommitTransaction([FromRoute] TransactionRef txn)
        {
            _logger.LogInformation("--COOR: transaction commit cmd received.");
            var result = await _coordinator.CommitTransaction(txn);
            _logger.LogInformation("--COOR: transaction committed.");
            return result ? Ok() : StatusCode(503);
        }

        [HttpPut("transactions/{txn}/rollback")]
        public async Task<IActionResult> RollbackTransaction([FromRoute] TransactionRef txn)
        {
            _logger.LogInformation("--COOR: rollback requested.");
            await _coordinator.RollbackTransaction(txn);
            _logger.LogInformation("--COOR: transaction rolled-back.");
            return Ok();
        }
    }
}
