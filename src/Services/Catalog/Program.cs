var builder = WebApplication.CreateBuilder(args);
var IF_ENABLED_IDENTITY = (Action doWithIdentity) =>
{
    if (string.IsNullOrWhiteSpace(builder.Configuration["DisableIdentity"]))
        doWithIdentity();
};

#region Register app services
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

builder.Services.AddScoped<FulfillmentMethodService>();
builder.Services.AddScoped<ListingService>();
builder.Services.AddTransient<IImageService, EventBasedImageService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));

builder.Services.RegisterEventBus(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityApi"];
        options.Audience = "catalog";
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasReadScope", policy =>
    {
        policy
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "catalog.read");
    });

    options.AddPolicy("HasModifyScope", policy =>
    {
        policy.RequireAuthenticatedUser()
            .RequireClaim("scope", "catalog.modify");
    });
});

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration["AllowedOrigins"]!.Split(',');
    options.AddDefaultPolicy(policy => policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader());
});
#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions
        .Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// DISABLES IDENTITY
IF_ENABLED_IDENTITY(() =>
{
    app.UseAuthentication();
    app.UseAuthorization();
});

app.MapControllers();
app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await catalogContext.Seed(scope.ServiceProvider);
}

app.ConfigureEventBus();

await app.RunAsync();

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
        services.AddTransient<OrderPlacedIntegrationEventHandler>();
        services.AddTransient<ProductOrdersStatusReportChangedIntegrationEventHandler>();
        services.AddTransient<ClientAccountClosedIntegrationEventHandler>();
        services.AddTransient<StockIssuedIntegrationEventHandler>();
        services.AddTransient<StockReceivedIntegrationEventHandler>();
        services.AddTransient<StockAdjustedIntegrationEventHandler>();
        services.AddTransient<ProductImageSavedIntegrationEventHandler>();
        services.AddTransient<ProductImageFailedToSaveIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<
            OrderPlacedIntegrationEvent,
            OrderPlacedIntegrationEventHandler>();
        eventBus.Subscribe<
            ProductOrdersStatusReportChangedIntegrationEvent,
            ProductOrdersStatusReportChangedIntegrationEventHandler>();
        eventBus.Subscribe<
            ClientAccountClosedIntegrationEvent,
            ClientAccountClosedIntegrationEventHandler>();
        eventBus.Subscribe<
            StockIssuedIntegrationEvent,
            StockIssuedIntegrationEventHandler>();
        eventBus.Subscribe<
            StockReceivedIntegrationEvent,
            StockReceivedIntegrationEventHandler>();
        eventBus.Subscribe<
            StockAdjustedIntegrationEvent,
            StockAdjustedIntegrationEventHandler>();
        eventBus.Subscribe<
            ProductImageSavedIntegrationEvent,
            ProductImageSavedIntegrationEventHandler>();
        eventBus.Subscribe<
            ProductImageFailedToSaveIntegrationEvent,
            ProductImageFailedToSaveIntegrationEventHandler>();
    }
}

