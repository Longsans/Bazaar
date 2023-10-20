using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrderingTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<OrderingDbContext>(options =>
        {
            options.UseSqlServer(
                "Server=.;Database=Ordering_Int_Tests;Trusted_Connection=True;Trust Server Certificate=True;");
        });

        services.AddTransient<EventBusTestDouble>();
    }
}
