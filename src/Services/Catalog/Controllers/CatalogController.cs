using Bazaar.Catalog.Model;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Catalog.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _catalogRepo.GetItemById(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("txn/{txn}/{productId}")]
        public IActionResult GetInTransaction(string productId, [FromRoute] TransactionRef txn)
        {
            var item = _catalogRepo.GetItemByProductId(productId);
            if (item == null)
                return NotFound();

            _catalogRm.LockReadIndex(txn, item.Id);
            return Ok(item);
        }

        [HttpPatch("txn/{txn}/{productId}")]
        public IActionResult UpdateStockInTransaction(string productId, [FromBody] int availableStock, [FromRoute] TransactionRef txn)
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
    }
}