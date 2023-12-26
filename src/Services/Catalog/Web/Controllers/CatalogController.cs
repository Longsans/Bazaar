namespace Bazaar.Catalog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly DeleteCatalogItemService _deleteService;
    private readonly ListingService _listingService;

    public CatalogController(
        ICatalogRepository catalogRepo,
        DeleteCatalogItemService deleteService,
        ListingService listingService)
    {
        _catalogRepo = catalogRepo;
        _deleteService = deleteService;
        _listingService = listingService;
    }

    [HttpGet("{id}")]
    //[Authorize(Policy = "HasReadScope")]
    public ActionResult<CatalogItemResponse> GetById(int id, bool includeDeleted = false)
    {
        var item = _catalogRepo.GetById(id);
        if (item == null || !includeDeleted && item.IsDeleted)
        {
            return NotFound();
        }

        return new CatalogItemResponse(item);
    }

    [HttpGet]
    //[Authorize(Policy = "HasReadScope")]
    public ActionResult<IEnumerable<CatalogItemResponse>> GetByCriteria(
        string? productId = null, string? sellerId = null,
        string? nameSubstring = null, bool includeDeleted = false)
    {
        if (string.IsNullOrWhiteSpace(productId) &&
            string.IsNullOrWhiteSpace(sellerId) &&
            string.IsNullOrWhiteSpace(nameSubstring))
        {
            return BadRequest("At least one of the following arguments must be specified: " +
                "productId, sellerId, partOfName");
        }
        if (!string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(sellerId))
        {
            return BadRequest("Only one of the following arguments can be specified: productId, sellerId.");
        }
        if (!string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(nameSubstring))
        {
            return BadRequest("The productId and partOfName arguments cannot be combined.");
        }

        var items = new List<CatalogItem>();
        if (!string.IsNullOrWhiteSpace(productId))
        {
            var error = AddByProductIds(items, productId);
            if (error.HasValue)
            {
                return NotFound(new { error.Value.productId, error.Value.error });
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(sellerId))
            {
                AddOrFilterBySellerIds(ref items, sellerId!);
            }
            if (!string.IsNullOrWhiteSpace(nameSubstring))
            {
                AddOrFilterByNameSubstring(ref items, nameSubstring!);
            }
        }

        if (!includeDeleted)
        {
            items = items.Where(i => !i.IsDeleted).ToList();
        }
        return items.Select(x => new CatalogItemResponse(x)).ToList();
    }

    // Shopper only
    [HttpPost]
    [Authorize(Policy = "HasModifyScope")]
    public ActionResult<CatalogItemResponse> Create(CreateCatalogItemRequest command)
    {
        var createdItem = _catalogRepo.Create(command.ToNewCatalogItem());
        return CreatedAtAction(nameof(GetById), new { createdItem.Id },
            new CatalogItemResponse(createdItem));
    }

    // Shopper only
    [HttpPut("{productId}/product-details")]
    [Authorize(Policy = "HasModifyScope")]
    public ActionResult<CatalogItemResponse> UpdateProductDetails(string productId,
        UpdateProductDetailsRequest request)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null || catalogItem.IsDeleted)
        {
            return NotFound();
        }

        catalogItem.ChangeProductDetails(
            request.Name, request.Description, request.Price);

        _catalogRepo.Update(catalogItem);

        return new CatalogItemResponse(catalogItem);
    }

    // Shopper only
    [HttpPatch("{productId}/stock")]
    public ActionResult<CatalogItemResponse> ChangeStock(string productId, ChangeStockRequest request)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null || catalogItem.IsDeleted)
        {
            return NotFound();
        }

        // This bit of business logic unfortunately can only be here
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

        _catalogRepo.Update(catalogItem);
        return new CatalogItemResponse(catalogItem);
    }

    // Shopper only
    [HttpPatch("{productId}/listing")]
    public IActionResult ChangeListingStatus(string productId, ChangeListingStatusRequest request)
    {
        var result = request.Status switch
        {
            ListingCloseStatus.Listed => _listingService.Relist(productId),
            ListingCloseStatus.Closed => _listingService.CloseListing(productId),
            _ => Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.Status),
                ErrorMessage = "Invalid status."
            })
        };
        return result.ToActionResult(this);
    }

    // Shopper only
    [HttpDelete("{productId}")]
    public IActionResult SoftDelete(string productId)
    {
        try
        {
            _deleteService.SoftDeleteByProductId(productId);
        }
        catch (Exception ex) when
            (ex is DeleteProductWithOrdersInProgressException
            || ex is DeleteFbbProductWhenFbbInventoryNotEmptyException)
        {
            return Conflict(new { error = ex.Message });
        }

        return NoContent();
    }

    #region Helpers
    private (string productId, string error)? AddByProductIds(List<CatalogItem> items, string joinedProductIds)
    {
        var productIds = joinedProductIds.Split(',');

        foreach (var id in productIds)
        {
            var item = _catalogRepo.GetByProductId(id);
            if (item == null)
            {
                return new(id, "Product not found.");
            }
            items.Add(item);
        }
        return null;
    }

    private void AddOrFilterBySellerIds(ref List<CatalogItem> items, string joinedSellerIds)
    {
        var sellerIds = joinedSellerIds.Split(',');

        if (items.Any())
        {
            items = items.Where(item => sellerIds.Contains(item.SellerId)).ToList();
        }

        foreach (var id in sellerIds)
        {
            items.AddRange(_catalogRepo.GetBySellerId(id));
        }
    }

    private void AddOrFilterByNameSubstring(ref List<CatalogItem> items, string nameSubstring)
    {
        if (items.Count == 0)
        {
            items.AddRange(_catalogRepo.GetByNameSubstring(nameSubstring));
        }
        else
        {
            items = items.Where(item => item.ProductName.Contains(nameSubstring)).ToList();
        }
    }
    #endregion
}