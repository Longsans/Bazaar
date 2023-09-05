Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");
var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    try
    {
        await Run();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Unhandled exception");
    }
    finally
    {
        Log.Information("Shut down complete");
        Log.CloseAndFlush();
    }
}
else
{
    await Run();
}

async Task Run()
{

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var app = await builder
        .ConfigureServices()
        .ConfigurePipeline();

    await app.RunAsync();
}