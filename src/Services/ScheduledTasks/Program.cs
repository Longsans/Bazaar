using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration["HangfireConnection"]));

builder.Services.AddHangfireServer();

// Background jobs
builder.Services.AddHttpClient<MarkOverdueUnfulfillableFbbInventoriesJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

using (var sqlConn = new SqlConnection(app.Configuration["DbServerConnection"]))
{
    var sql = @"
        USE master;

        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @dbName)
        BEGIN
            CREATE DATABASE [@dbName];
        END";
    var command = new SqlCommand(sql, sqlConn);
    command.Parameters.AddWithValue("@dbName", "Hangfire");
    sqlConn.Open();
    command.ExecuteNonQuery();
    app.Logger.LogInformation("Hangfire database created.");
}

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate(nameof(MarkOverdueUnfulfillableFbbInventoriesJob),
    () => app.Services
        .GetRequiredService<MarkOverdueUnfulfillableFbbInventoriesJob>()
        .ExecuteAsync(),
    Cron.Daily);

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHangfireDashboard();

app.Run();
