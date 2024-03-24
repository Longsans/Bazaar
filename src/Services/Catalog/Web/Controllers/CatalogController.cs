namespace Bazaar.Catalog.Web.Controllers;
using System;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly IRepository<ProductCategory> _categoryRepo;
    private readonly ListingService _listingService;
    private readonly IImageService _imgService;
    private readonly IEventBus _eventBus;

    public CatalogController(
        IRepository<CatalogItem> catalogRepo,
        IRepository<ProductCategory> categoryRepo,
        ListingService listingService,
        IImageService imgService,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _categoryRepo = categoryRepo;
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
        int? categoryId, string? sellerId, string? nameSubstring, bool includeDeleted)
    {
        try
        {
            var spec = new CatalogItemsByDetailCriteriaSpec(categoryId, sellerId, nameSubstring, includeDeleted);
            var items = await _catalogRepo.ListAsync(spec);
            return items.Select(x => new CatalogItemResponse(x, _imgService.ImageHostLocation)).ToList();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("/api/catalog-by-department/{departmentId}")]
    public async Task<ActionResult<IEnumerable<CatalogItemResponse>>> GetByMainDepartmentAndName(int departmentId, string? nameSubstring, bool includeDeleted)
    {
        var items = await _catalogRepo.ListAsync(
            new CatalogItemsByMainDepartmentAndNameSpec(departmentId, nameSubstring, includeDeleted));
        return items.Select(x => new CatalogItemResponse(x, _imgService.ImageHostLocation)).ToList();
    }

    [HttpGet("/api/catalog-by-subcategory/{subcategoryId}")]
    public async Task<ActionResult<IEnumerable<CatalogItemResponse>>> GetBySubcategoryAndName(int subcategoryId, string? nameSubstring, bool includeDeleted)
    {
        var items = await _catalogRepo.ListAsync(
            new CatalogItemsBySubcategoryAndNameSpec(subcategoryId, nameSubstring, includeDeleted));
        return items.Select(x => new CatalogItemResponse(x, _imgService.ImageHostLocation)).ToList();
    }

    [HttpPatch("stock")]
    public async Task<ActionResult<IEnumerable<CatalogItemResponse>>> BulkUpdateProductStock(IEnumerable<UpdateStockRequest> request)
    {
        var allItemsUnique = request.Select(x => x.ProductId)
            .GroupBy(x => x)
            .Select(g => g.Count())
            .All(c => c == 1);
        if (!allItemsUnique)
            return BadRequest(new
            {
                error = "Bulk update request contains duplicate product listings."
            });

        var requestedIds = request.Select(x => x.ProductId);
        var items = await _catalogRepo.ListAsync(new CatalogItemsByProductIdSpec(requestedIds));
        var retrievedIds = items.Select(x => x.ProductId);
        var notFoundIds = requestedIds.Except(retrievedIds);
        if (notFoundIds.Any())
        {
            return NotFound(new
            {
                error = $"Bulk update request contains items with the following IDs, of which the corresponding items were not found: {string.Join(", ", notFoundIds)}"
            });
        }

        foreach ((var item, var update) in items.Zip(request))
        {
            try
            {
                item.UpdateStock(update.Units);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (ManualFbbStockManagementNotSupportedException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        await _catalogRepo.UpdateRangeAsync(items);
        return items.Select(x => new CatalogItemResponse(x, _imgService.ImageHostLocation)).ToList();
    }

    #region Seller only endpoints
    [HttpPost]
    //[Authorize(Policy = "HasModifyScope")]
    public async Task<ActionResult<CatalogItemResponse>> Create([FromForm] CreateCatalogItemRequest request)
    {
        var category = await _categoryRepo.SingleOrDefaultAsync(
            new ProductCategoryByIdSpec(request.SubcategoryId));
        if (category is null)
        {
            return NotFound("Product category not found.");
        }

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
        var catalogItem = new CatalogItem(request.Name, request.Description, request.Price, request.AvailableStock, category,
            request.ProductLength, request.ProductWidth, request.ProductHeight, request.SellerId, request.FulfillmentMethod);
        await _catalogRepo.AddAsync(catalogItem);

        var imageUri = await _imgService.SaveImageForProduct(catalogItem.ProductId, image);
        if (imageUri is not null)
        {
            catalogItem.ChangeProductDetails(imageFilename: imageUri);
            await _catalogRepo.UpdateAsync(catalogItem);
        }
        _eventBus.Publish(new CatalogItemCreatedIntegrationEvent(catalogItem));
        return CreatedAtAction(nameof(GetByProductId), new { productId = catalogItem.ProductId },
            new CatalogItemResponse(catalogItem, _imgService.ImageHostLocation));
    }

    [HttpPatch("{productId}")]
    public async Task<ActionResult<CatalogItemResponse>> UpdateProductListing(string productId, UpdateProductListingRequest request)
    {
        var spec = new CatalogItemByProductIdSpec(productId);
        var catalogItem = await _catalogRepo.FirstOrDefaultAsync(spec);
        if (catalogItem == null)
        {
            return NotFound("Catalog item not found.");
        }

        try
        {
            catalogItem.ChangeProductDetails(request.Name, request.Description, request.Price, request.ImageUrl);
            if (request.Dimensions is not null)
            {
                catalogItem.ChangeProductDimensions(request.Dimensions.LengthInCm, request.Dimensions.WidthInCm, request.Dimensions.HeightInCm);
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new {error = ex.Message});
        }
        await _catalogRepo.UpdateAsync(catalogItem);
        return new CatalogItemResponse(catalogItem, _imgService.ImageHostLocation);
    }

    [HttpPatch]
    public async Task<ActionResult<IEnumerable<CatalogItemResponse>>> BulkUpdateProductListings(IEnumerable<UpdateProductListingRequest> request)
    {
        var allItemsUnique = request.Select(x => x.ProductId)
            .GroupBy(x => x)
            .Select(g => g.Count())
            .All(c => c == 1);
        if (!allItemsUnique)
            return BadRequest(new
            {
                error = "Bulk update request contains duplicate product listings."
            });

        var requestedIds = request.Select(x => x.ProductId);
        var items = await _catalogRepo.ListAsync(new CatalogItemsByProductIdSpec(requestedIds));
        var retrievedIds = items.Select(x => x.ProductId);
        var notFoundItems = requestedIds.Except(retrievedIds);
        if (notFoundItems.Any())
        {
            return NotFound(new
            {
                error = $"Bulk update request contains items with the following IDs, of which the corresponding items were not found: {string.Join(", ", notFoundItems)}"
            });
        }

        try
        {
            foreach ((var item, var update) in items.Zip(request))
            {
                item.ChangeProductDetails(update.Name, update.Description, update.Price, update.ImageUrl);
                var dimensions = update.Dimensions;
                if (dimensions is not null)
                    item.ChangeProductDimensions(dimensions.LengthInCm, dimensions.WidthInCm, dimensions.HeightInCm);
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new {error = ex.Message});
        }
        await _catalogRepo.UpdateRangeAsync(items);
        return items.Select(x => new CatalogItemResponse(x, _imgService.ImageHostLocation)).ToList();
    }

    [HttpPatch("{productId}/listing-status")]
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

    [HttpPatch("{productId}/category")]
    public async Task<ActionResult<CatalogItemResponse>> ChangeSubcategory(string productId, ChangeProductSubcategoryRequest request)
    {
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(new CatalogItemByProductIdSpec(productId));
        if (catalogItem is null)
        {
            return NotFound("Catalog item not found.");
        }
        var category = await _categoryRepo.SingleOrDefaultAsync(new ProductCategoryByIdSpec(request.SubcategoryId));
        if (category is null)
        {
            return NotFound("Product category not found.");
        }
        catalogItem.ChangeSubcategory(category);
        await _catalogRepo.UpdateAsync(catalogItem);
        return new CatalogItemResponse(catalogItem, _imgService.ImageHostLocation);
    }
    #endregion
}