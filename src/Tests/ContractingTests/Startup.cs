﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContractingTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ContractingDbContext>(options =>
        {
            options.UseInMemoryDatabase("ContractingTestDb");
        });

        services.AddTransient<IPartnerRepository, PartnerRepository>();
        services.AddTransient<IContractRepository, ContractRepository>();
        services.AddTransient<ContractController>();
    }
}