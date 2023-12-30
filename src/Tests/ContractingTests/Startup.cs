using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContractingTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ContractingDbContext>(options =>
        {
            options.UseSqlServer(
                "Server=.;Database=Bazaar_Contracting_Tests;Trusted_Connection=True;Trust Server Certificate=True;");
        });

        services.AddTransient<EventBusTestDouble>();
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddLogging();
    }
}
