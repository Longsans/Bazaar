namespace Bazaar.Catalog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly ListingService _listingService;

    public CatalogController(
        IRepository<CatalogItem> catalogRepo,
        ListingService listingService)
    {
        _catalogRepo = catalogRepo;
        _listingService = listingService;
    }

    [HttpGet("{productId}")]
    //[Authorize(Policy = "HasReadScope")]
    public async Task<ActionResult<CatalogItemResponse>> GetByProductId(string productId, bool includeDeleted = false)
    {
        var item = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId, includeDeleted));
        if (item == null)
        {
            return NotFound();
        }

        return new CatalogItemResponse(item);
    }

    [HttpGet]
    //[Authorize(Policy = "HasReadScope")]
    public async Task<ActionResult<IEnumerable<CatalogItemResponse>>> GetByCriteria(
        string? productId = null, string? sellerId = null,
        string? nameSubstring = null, bool includeDeleted = false)
    {
        if (string.IsNullOrWhiteSpace(productId) &&
            string.IsNullOrWhiteSpace(sellerId) &&
            string.IsNullOrWhiteSpace(nameSubstring))
        {
            return BadRequest(new
            {
                error = "At least one of the following arguments must be specified: " +
                    "productId, sellerId, partOfName"
            });
        }
        if (!string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(sellerId))
        {
            return BadRequest(new
            {
                error = "Only one of the following arguments can be specified: productId, sellerId."
            });
        }
        if (!string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(nameSubstring))
        {
            return BadRequest(new
            {
                error = "The productId and partOfName arguments cannot be combined."
            });
        }

        var items = new List<CatalogItem>();
        if (!string.IsNullOrWhiteSpace(productId))
        {
            var spec = new CatalogItemByProductIdSpec(productId, includeDeleted);
            var item = await _catalogRepo.SingleOrDefaultAsync(spec);
            if (item != null)
            {
                items.Add(item);
            }
        }
        else
        {
            Specification<CatalogItem>? spec = null;
            if (!string.IsNullOrWhiteSpace(sellerId))
            {
                spec = new CatalogItemsBySellerIdSpec(sellerId, includeDeleted);
            }
            if (!string.IsNullOrWhiteSpace(nameSubstring))
            {
                var nameSpec = new CatalogItemsByNameSubstringSpec(nameSubstring!, includeDeleted);
                if (spec is not null)
                {
                    ((List<WhereExpressionInfo<CatalogItem>>)spec.WhereExpressions)
                        .Add(nameSpec.WhereExpressions.First());
                }
                else
                {
                    spec = nameSpec;
                }
            }
            items.AddRange(await _catalogRepo.ListAsync(spec!));
        }
        return items.Select(x => new CatalogItemResponse(x)).ToList();
    }

    // Shopper only
    [HttpPost]
    [Authorize(Policy = "HasModifyScope")]
    public async Task<ActionResult<CatalogItemResponse>> Create(CreateCatalogItemRequest command)
    {
        var createdItem = await _catalogRepo.AddAsync(command.ToNewCatalogItem());
        return CreatedAtAction(nameof(GetByProductId), new { createdItem.Id },
            new CatalogItemResponse(createdItem));
    }

    // Shopper only
    [HttpPatch("{productId}")]
    [Authorize(Policy = "HasModifyScope")]
    public async Task<ActionResult<CatalogItemResponse>> UpdateProductDetails(string productId,
        UpdateProductDetailsRequest request)
    {
        var spec = new CatalogItemByProductIdSpec(productId);
        var catalogItem = await _catalogRepo.FirstOrDefaultAsync(spec);
        if (catalogItem == null)
        {
            return NotFound();
        }

        catalogItem.ChangeProductDetails(
            request.Name, request.Description, request.Price);
        await _catalogRepo.UpdateAsync(catalogItem);
        return new CatalogItemResponse(catalogItem);
    }

    // Shopper only
    [HttpPatch("{productId}/stock")]
    public async Task<ActionResult<CatalogItemResponse>> ChangeStock(string productId, ChangeStockRequest request)
    {
        var spec = new CatalogItemByProductIdSpec(productId);
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(spec);
        if (catalogItem == null)
        {
            return NotFound();
        }

        // This bit of business logic being here is fine for the moment
        if (catalogItem.IsFbb)
        {
            return Conflict(new
            {
                error = "Cannot change FBB products' stocks directly. " +
                    "FBB stocks are updated by Bazaar."
            });
        }

        try
        {
            if (request.ChangeType == StockChangeType.Reduce)
                catalogItem.ReduceStock(request.Units);
            else if (request.ChangeType == StockChangeType.Restock)
                catalogItem.Restock(request.Units);
            else throw new ArgumentException("Change type can only be Reduce or Restock.");
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is NotEnoughStockException
            || ex is ExceedingMaxStockThresholdException)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _catalogRepo.UpdateAsync(catalogItem);
        return new CatalogItemResponse(catalogItem);
    }

    // Seller only
    [HttpPatch("{productId}/listing")]
    public async Task<IActionResult> ChangeListingStatus(string productId, ChangeListingStatusRequest request)
    {
        var result = request.Status switch
        {
            RequestedListingStatus.Listed => await _listingService.Relist(productId),
            RequestedListingStatus.Closed => await _listingService.CloseListing(productId),
            RequestedListingStatus.Deleted => await _listingService.DeleteListing(productId),
            _ => Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.Status),
                ErrorMessage = "Invalid status."
            })
        };
        return result.ToActionResult(this);
    }
}