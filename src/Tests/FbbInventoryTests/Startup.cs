using Microsoft.Extensions.DependencyInjection;

namespace FbbInventoryTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<EventBusTestDouble>();
    }
}
