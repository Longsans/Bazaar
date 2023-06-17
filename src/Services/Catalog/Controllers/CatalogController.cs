using Bazaar.Catalog.Model;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Catalog.Controllers
{
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepo;
        private readonly IResourceManager<CatalogItem, int> _catalogRm;

        public CatalogController(ICatalogRepository catalogRepo, IResourceManager<CatalogItem, int> catalogRm)
        {
            _catalogRepo = catalogRepo;
            _catalogRm = catalogRm;
        }

        [HttpGet("api/catalog/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _catalogRepo.GetItemById(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("api/catalog/{productId}/stock")]
        public ActionResult<int> GetAvailableStockInTransaction(string productId, [FromQuery] TransactionRef txn)
        {
            var item = _catalogRepo.GetItemByProductId(productId);
            if (item == null)
                return NotFound();

            _catalogRm.LockReadIndex(txn, item.Id);
            return Ok(item.AvailableStock);
        }

        [HttpPatch("api/txn/{txn}/catalog/{productId}")]
        public IActionResult UpdateStockInTransaction(string productId, [FromRoute] TransactionRef txn, [FromBody] int availableStock)
        {
            var item = _catalogRepo.GetItemByProductId(productId);
            if (item == null)
                return NotFound();

            var txnState = _catalogRm.GetOrCreateTransactionState(txn);
            var update = new CatalogItem(item)
            {
                AvailableStock = availableStock,
            };
            txnState.PendingUpdates.Add(update);
            return Ok(update);
        }

        [HttpPost("api/txn")]
        public IActionResult PrepareToCommitTransaction([FromBody] TransactionRef txn)
        {
            _catalogRm.HandlePrepare(txn);
            return Ok();
        }

        [HttpPut("api/txn/{txn}")]
        public IActionResult CommitTransaction([FromRoute] TransactionRef txn, [FromBody] bool commit)
        {
            if (!commit)
            {
                _catalogRm.HandleRollback(txn);
                return Ok();
            }
            _catalogRm.HandleCommit(txn);
            return Ok();
        }
    }
}