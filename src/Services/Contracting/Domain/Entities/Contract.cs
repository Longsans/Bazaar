namespace Bazaar.Contracting.Domain.Entities;

public class Contract
{
    public int Id { get; private set; }

    public Client Client { get; private set; }
    public int ClientId { get; private set; }

    public SellingPlan SellingPlan { get; private set; }
    public int SellingPlanId { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public bool Ended => EndDate != null;

    // EF Core reads also use this constructor
    public Contract(int clientId, int sellingPlanId)
    {
        ClientId = clientId;
        SellingPlanId = sellingPlanId;
        StartDate = DateTime.Now.Date;
    }

    public void End()
    {
        if (EndDate != null)
            throw new ContractEndedException();

        EndDate = DateTime.Now.Date;
    }
}