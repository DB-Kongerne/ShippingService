using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

try
{
    logger.Debug("init main");
    
    // Create the builder and configure services
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();  // Use NLog as the logging provider

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();  // Run the application
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;  // Re-throws the caught exception to stop the application
}
finally
{
    // Ensure that NLog shuts down properly at the end of the application
    NLog.LogManager.Shutdown();
}
