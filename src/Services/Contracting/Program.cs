using Bazaar.Contracting.Application.IntegrationEvents;

var builder = WebApplication.CreateBuilder(args);
var IF_IDENTITY_ELSE = (Action doWithIdentity, Action doWithoutIdentity) =>
{
    if (string.IsNullOrWhiteSpace(builder.Configuration["DisableIdentity"]))
    {
        doWithIdentity();
    }
    else
    {
        doWithoutIdentity();
    }
};

#region Register app services
builder.Services.AddDbContext<ContractingDbContext>(options =>
{
    //builder.Configuration["ConnectionString"] =
    //@"Server=.;Database=Bazaar;Trusted_Connection=True;TrustServerCertificate=True;";
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

// Domain services
builder.Services.AddScoped<UpdateClientEmailAddressService>();
builder.Services.AddScoped<CloseClientAccountService>();

// Data services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(_ => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));

// Event bus
builder.Services.RegisterEventBus(builder.Configuration);

// AuthN and AuthZ
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityApi"];
        options.Audience = "contracting";
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasContractingScope", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "contracting"));
});
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DISABLES IDENTITY
IF_IDENTITY_ELSE(
    () =>
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization("HasContractingScope");
    },
    () => app.MapControllers()
);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContractingDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.ConfigureEventBus();

app.Run();

public static class EventBusExtensionMethods
{
    public static void RegisterEventBus(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
            var factory = new ConnectionFactory()
            {
                HostName = config["EventBusConnection"],
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(config["EventBusUserName"]))
            {
                factory.UserName = config["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(config["EventBusPassword"]))
            {
                factory.Password = config["EventBusPassword"];
            }

            var retryCount = 5;
            if (!string.IsNullOrEmpty(config["EventBusRetryCount"]))
            {
                retryCount = int.Parse(config["EventBusRetryCount"]!);
            }

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = config["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(config["EventBusRetryCount"]))
            {
                retryCount = int.Parse(config["EventBusRetryCount"]!);
            }

            return new EventBusRabbitMQ(
                rabbitMQPersistentConnection,
                logger,
                sp,
                eventBusSubcriptionsManager,
                subscriptionClientName,
                retryCount);
        });
        services.AddTransient<ProductListingsDeleteFailedIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<
            ProductListingsDeleteFailedIntegrationEvent,
            ProductListingsDeleteFailedIntegrationEventHandler>();
    }
}