namespace WebShoppingUI.DTOs;

public class Contract
{
    public int Id { get; }
    public int ClientId { get; }
    public int SellingPlanId { get; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }

    public Contract(int id, int clientId, int sellingPlanId, DateTime startDate, DateTime? endDate)
    {
        Id = id;
        ClientId = clientId;
        SellingPlanId = sellingPlanId;
        StartDate = startDate;
        EndDate = endDate;
    }
}
