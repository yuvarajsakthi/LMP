using Serilog;
using Kanini.LMP.Api.Extensions;
using Kanini.LMP.Application.Extensions;
using Kanini.LMP.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Clear default .NET logging
builder.Logging.ClearProviders();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console(outputTemplate: "{Level:u3}: {Message:lj}{NewLine}")
    .WriteTo.File(
        path: "Logs/lmp-log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )
    .CreateLogger();

// Replace default logging with Serilog
builder.Host.UseSerilog();


// Layer registrations
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddApiLayer(builder.Configuration);


var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex) when (ex.Message.Contains("JSON"))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid JSON format");
    }
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
