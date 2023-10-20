using Bazaar.Basket.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BasketTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<BasketDbContext>(options =>
        {
            options.UseSqlServer(
                "Server=.;Database=Bazaar_Basket_Tests;Trusted_Connection=True;Trust Server Certificate=True;");
        });

        services.AddTransient<EventBusTestDouble>();
    }
}
