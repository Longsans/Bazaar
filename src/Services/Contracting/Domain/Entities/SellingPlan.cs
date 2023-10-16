using Newtonsoft.Json;

namespace Bazaar.Contracting.Domain.Entities;

public class SellingPlan
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public decimal MonthlyFee { get; private set; }
    public decimal PerSaleFee { get; private set; }
    public float RegularPerSaleFeePercent { get; private set; }

    [JsonConstructor]
    public SellingPlan(
        string name,
        decimal monthlyFee,
        decimal perSaleFee,
        float regularPerSaleFeePercent)
    {
        Name = name;
        MonthlyFee = monthlyFee;
        PerSaleFee = perSaleFee;
        RegularPerSaleFeePercent = regularPerSaleFeePercent;
    }

    public SellingPlan(
        int id,
        string name,
        decimal monthlyFee,
        decimal perSaleFee,
        float regularPerSaleFeePercent) : this(
            name, monthlyFee, perSaleFee, regularPerSaleFeePercent)
    {
        Id = id;
    }

    public void ChangeFees(
        decimal monthlyFee, decimal perSaleFee, float regularPerSaleFeePercent)
    {
        if (monthlyFee <= 0m && perSaleFee <= 0m)
            throw new MonthlyAndPerSaleFeesNotPositiveException();

        if (regularPerSaleFeePercent <= 0f)
            throw new RegularPerSaleFeePercentNotPositiveException();

        MonthlyFee = monthlyFee;
        PerSaleFee = perSaleFee;
        RegularPerSaleFeePercent = regularPerSaleFeePercent;
    }
}