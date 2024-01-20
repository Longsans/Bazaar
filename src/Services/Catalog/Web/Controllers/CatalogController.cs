namespace Bazaar.Catalog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly ListingService _listingService;
    private readonly IImageService _imgService;
    private readonly IEventBus _eventBus;

    public CatalogController(
        IRepository<CatalogItem> catalogRepo,
        ListingService listingService,
        IImageService imgService,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _listingService = listingService;
        _imgService = imgService;
        _eventBus = eventBus;
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

        return new CatalogItemResponse(item, _imgService.ImageHostLocation);
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
        return items.Select(x => new CatalogItemResponse(x, _imgService.ImageHostLocation)).ToList();
    }

    // Seller only
    [HttpPost]
    //[Authorize(Policy = "HasModifyScope")]
    public async Task<ActionResult<CatalogItemResponse>> Create([FromForm] CreateCatalogItemRequest request)
    {
        Image? image;
        try
        {
            image = await Image.LoadAsync(request.Image.OpenReadStream());
        }
        catch (UnknownImageFormatException)
        {
            return BadRequest("Product image is not valid format.");
        }
        catch (InvalidImageContentException)
        {
            return BadRequest("Product image content is invalid.");
        }
        var catalogItem = new CatalogItem(
            request.Name, request.Description, request.Price,
            request.AvailableStock, request.SellerId, request.FulfillmentMethod);
        await _catalogRepo.AddAsync(catalogItem);

        var imageUri = await _imgService.SaveImageForProduct(catalogItem.ProductId, image);
        if (imageUri is not null)
        {
            catalogItem.ChangeProductDetails(imageFilename: imageUri);
            await _catalogRepo.UpdateAsync(catalogItem);
        }
        return CreatedAtAction(nameof(GetByProductId), new { productId = catalogItem.ProductId },
            new CatalogItemResponse(catalogItem, _imgService.ImageHostLocation));
    }

    // Seller only
    [HttpPatch("{productId}")]
    [Authorize(Policy = "HasModifyScope")]
    public async Task<ActionResult<CatalogItemResponse>> UpdateProductDetails(
        string productId, UpdateProductDetailsRequest request)
    {
        var spec = new CatalogItemByProductIdSpec(productId);
        var catalogItem = await _catalogRepo.FirstOrDefaultAsync(spec);
        if (catalogItem == null)
        {
            return NotFound();
        }

        catalogItem.ChangeProductDetails(request.Name, request.Description, request.Price, request.ImageUrl);
        await _catalogRepo.UpdateAsync(catalogItem);
        return new CatalogItemResponse(catalogItem, _imgService.ImageHostLocation);
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
        return new CatalogItemResponse(catalogItem, _imgService.ImageHostLocation);
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