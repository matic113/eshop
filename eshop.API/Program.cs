using eshop.Application;
using eshop.Infrastructure;
using FastEndpoints;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

var dashboardOrigins = builder.Configuration.GetSection("Cors:DashboardOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.WithOrigins(dashboardOrigins!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("eshop.API"))
    .WithTracing(t =>
    {
        t
         .AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddEntityFrameworkCoreInstrumentation();

        t.AddOtlpExporter(config => {
            config.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]!);
            config.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
    });

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapOpenApi();
app.MapScalarApiReference();


await app.ConfigurePersistence();

app.UseHttpsRedirection();

app.UseCors("AllowDashboard");

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.MapGet("/", () => "Welcome to the eShop API!");

app.Run();
