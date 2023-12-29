namespace Bazaar.Contracting.Infrastructure.Database;

public static class ContractingDbSeeding
{
    private const string PARTNERS_SECTION = "clients";
    private const string SELLING_PLANS_SECTION = "sellingPlans";

    public static async Task Seed(this ContractingDbContext context, IServiceProvider sp)
    {
        context.Database.EnsureDeleted();
        await context.Database.MigrateAsync();

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.SellingPlans.AddRange(adapter.ReadToObjects<SellingPlan>(SELLING_PLANS_SECTION));
        await context.SaveChangesAsync();

        context.Clients.AddRange(adapter.ReadToObjects<Client>(PARTNERS_SECTION));
        await context.SaveChangesAsync();
    }
}
