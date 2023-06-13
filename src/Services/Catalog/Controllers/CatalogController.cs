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

        [HttpGet("txn/{txn}")]
        public IActionResult GetInTransaction(int id, TransactionRef txn)
        {
            var item = _catalogRepo.GetItemById(id);
            if (item == null)
                return NotFound();

            var txnState = GetOrCreateState(txn);
            txnState.ReadIndexes.Add(id);
            return Ok(item);
        }

        [HttpPost("txn/{txn}")]
        public IActionResult UpdateInTransaction(CatalogItem item, TransactionRef txn)
        {
            var txnState = GetOrCreateState(txn);
            txnState.PendingUpdates.Add(item);
            return Ok(txnState);
        }

        private TransactionState<CatalogItem, int> GetOrCreateState(TransactionRef txn)
        {
            var state = _catalogRm.OngoingTransactions[txn];
            if (state == null)
            {
                state = new TransactionState<CatalogItem, int>(item => item.Id);
                _catalogRm.OngoingTransactions.Add(txn, state);
            }
            return state;
        }
    }
}