namespace Bazaar.Catalog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IDeleteCatalogItemService _deleteService;

    public CatalogController(
        ICatalogRepository catalogRepo,
        IDeleteCatalogItemService deleteService)
    {
        _catalogRepo = catalogRepo;
        _deleteService = deleteService;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "HasReadScope")]
    public IActionResult GetById(int id, bool includeDeleted = false)
    {
        var item = _catalogRepo.GetById(id);
        if (item == null || !includeDeleted && item.IsDeleted)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpGet]
    [Authorize(Policy = "HasReadScope")]
    public ActionResult<IEnumerable<CatalogItem>> GetByCriteria(
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

        return includeDeleted
            ? items
            : items.Where(i => !i.IsDeleted).ToList();
    }

    [HttpPost]
    [Authorize(Policy = "HasModifyScope")]
    public ActionResult<CatalogItem> Create(CreateCatalogItemRequest command)
    {
        var createdItem = _catalogRepo.Create(command.ToNewCatalogItem());
        return CreatedAtAction(nameof(GetById), new { createdItem.Id }, createdItem);
    }

    [HttpPut("{productId}/product-details")]
    [Authorize(Policy = "HasModifyScope")]
    public ActionResult<CatalogItem> UpdateProductDetails(string productId,
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

        return catalogItem;
    }

    [HttpPut("{productId}/stock")]
    public ActionResult<CatalogItem> Restock(string productId, RestockRequest request)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null || catalogItem.IsDeleted)
        {
            return NotFound();
        }

        try
        {
            catalogItem.Restock(request.RestockUnits);
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is ExceedingMaxStockThresholdException)
        {
            return BadRequest(new { error = ex.Message });
        }

        return catalogItem;
    }

    [HttpPut("{productId}/stock-thresholds")]
    public ActionResult<CatalogItem> UpdateStockThresholds(
        string productId, UpdateStockThresholdsRequest request)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null || catalogItem.IsDeleted)
        {
            return NotFound();
        }

        try
        {
            catalogItem.ChangeStockThresholds(
                request.RestockThreshold, request.MaxStockThreshold);
        }
        catch (Exception ex) when (
            ex is ArgumentException
            || ex is ExceedingMaxStockThresholdException)
        {
            return BadRequest(new { error = ex.Message });
        }

        return catalogItem;
    }

    [HttpDelete("{productId}")]
    public IActionResult SoftDelete(string productId)
    {
        return _deleteService.SoftDeleteByProductId(productId)
            .ToActionResult(this);
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