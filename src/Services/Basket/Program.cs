using RabbitMQ.Client;

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
builder.Services.AddDbContext<BasketDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
    options.UseTriggers(triggerOptions =>
    {
        triggerOptions.AddTrigger<BasketItemChangeTrigger>();
    });
});

builder.Services.AddScoped<IBasketCheckoutService, BasketCheckoutService>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));

builder.Services.RegisterEventBus(builder.Configuration);

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityApi"];
        options.Audience = "basket";
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("HasBasketScope", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "basket"));
});
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

        app.MapControllers().RequireAuthorization("HasBasketScope");
    },
    () => app.MapControllers()
);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
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
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {

    }
}