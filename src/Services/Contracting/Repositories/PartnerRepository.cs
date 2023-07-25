namespace Bazaar.Contracting.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly Partner[] _partners;

    public PartnerRepository()
    {
        var partner = new Partner
        {
            Id = 1,
            ExternalId = "PNER-1",
            FirstName = "Long",
            LastName = "Do",
            Email = "philongdo01@gmail.com",
            PhoneNumber = "0901234567",
        };
        var sellingPlan = new SellingPlan
        {
            Id = 1,
            Name = "Individual",
            MonthlyFee = 0.0m,
            PerSaleFee = 0.99m,
            RegularPerSaleFeePercent = 0.1f,
        };
        var contract = new Contract
        {
            Id = 1,
            PartnerId = 1,
            Partner = partner,
            SellingPlanId = 1,
            SellingPlan = sellingPlan,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today + TimeSpan.FromDays(180),
        };
        partner.Contracts.Add(contract);
        _partners = new Partner[] {
            partner
        };
    }

    public Partner? GetByExternalId(string externalId)
    {
        return _partners.FirstOrDefault(p => p.ExternalId == externalId);
    }

    public Partner? GetById(int id)
    {
        return _partners.FirstOrDefault(p => p.Id == id);
    }
}