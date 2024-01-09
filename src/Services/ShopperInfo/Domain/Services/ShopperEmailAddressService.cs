namespace ShopperInfo.Domain.Services;

public class ShopperEmailAddressService
{
    private readonly IShopperRepository _shopperRepo;

    public ShopperEmailAddressService(IShopperRepository shopperRepo)
    {
        _shopperRepo = shopperRepo;
    }

    public void ChangeEmailAddress(string shopperId, string newEmailAddress)
    {
        var shopper = _shopperRepo.GetByExternalId(shopperId)
            ?? throw new KeyNotFoundException("Shopper not found.");
        var emailAddressOwner = _shopperRepo.GetByEmailAddress(newEmailAddress);
        if (emailAddressOwner is not null)
        {
            if (emailAddressOwner.ExternalId != shopper.ExternalId)
            {
                throw new DuplicateEmailAddressException();
            }
            return;
        }
        shopper.ChangeEmailAddress(newEmailAddress);
        _shopperRepo.Update(shopper);
    }
}
