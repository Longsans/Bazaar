using CatalogTests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseSqlServer(
                "Server=.;Database=Bazaar_Catalog_Tests;Trusted_Connection=True;Trust Server Certificate=True;");
        });

        services.AddTransient<EventBusTestDouble>();
    }
}
