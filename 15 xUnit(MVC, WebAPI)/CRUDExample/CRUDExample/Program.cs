using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;
using Serilog;
using CRUDExample.Filters.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
  loggerConfiguration
      .ReadFrom.Configuration(context.Configuration)      //read configuration settings from built-in Iconfiguration
      .ReadFrom.Services(services);           //Read out current app's service and make them available to serilog
});

builder.Services.AddTransient<PersonsListActionFilter>();

// Register HTTP logging services
builder.Services.AddHttpLogging(options =>
{
  options.LoggingFields =
      Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
      Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});

builder.Services.AddTransient<ResponseHeaderActionFilter>();

//It adds controllers and views as services
builder.Services.AddControllersWithViews(options =>
{
  //options.Filters.Add<ResponseHeaderActionFilter>();

  var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

  options.Filters.Add(new ResponseHeaderActionFilter(logger)
  {
    Key = "My-Key-From-Global",
    Value = "My-Value-From-Global",
    Order = 2
  });
});

//add services into IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepositoy>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
