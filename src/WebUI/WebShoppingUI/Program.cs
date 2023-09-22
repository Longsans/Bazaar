var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

app.Run();
