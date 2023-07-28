namespace Bazaar.Contracting.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly List<Partner> _partners;
    private int _nextId => _partners.Count + 1;

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
            Gender = Gender.Male,
            DateOfBirth = new DateTime(2001, 12, 11),
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
        _partners = new List<Partner> {
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

    public Partner Create(Partner partner)
    {
        partner.Id = _nextId;
        partner.ExternalId = $"PNER-{_nextId}";
        _partners.Add(partner);
        return partner;
    }

    public bool Update(Partner update)
    {
        var partner = _partners.FirstOrDefault(p => p.Id == update.Id);
        if (partner == null)
        {
            return false;
        }
        update.ExternalId = partner.ExternalId;
        _partners.Remove(partner);
        _partners.Add(update);
        return true;
    }

    public bool Delete(int id)
    {
        var toRemove = _partners.FirstOrDefault(p => p.Id == id);
        if (toRemove == null)
        {
            return false;
        }
        _partners.Remove(toRemove);
        return true;
    }
}