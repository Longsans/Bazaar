namespace WebShoppingUI.DTOs;

public class SellingPlan
{
    public int Id { get; }
    public string Name { get; }
    public decimal MonthlyFee { get; }
    public decimal PerSaleFee { get; }
    public float RegularPerSaleFeePercent { get; }

    public SellingPlan(int id, string name, decimal monthlyFee, decimal perSaleFee, float regularPerSaleFeePercent)
    {
        Id = id;
        Name = name;
        MonthlyFee = monthlyFee;
        PerSaleFee = perSaleFee;
        RegularPerSaleFeePercent = regularPerSaleFeePercent;
    }
}
