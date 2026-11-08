using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
  public static class ConfigureServicesExtension
  {
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddTransient<PersonsListActionFilter>();

      // Register HTTP logging services
      services.AddHttpLogging(options =>
      {
        options.LoggingFields =
            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
      });

      services.AddTransient<ResponseHeaderActionFilter>();

      //It adds controllers and views as services
      services.AddControllersWithViews(options =>
      {
        //options.Filters.Add<ResponseHeaderActionFilter>();

        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

        options.Filters.Add(new ResponseHeaderActionFilter(logger)
        {
          Key = "My-Key-From-Global",
          Value = "My-Value-From-Global",
          Order = 2
        });
      });

      //add services into IoC container
      services.AddScoped<ICountriesService, CountriesService>();
      services.AddScoped<IPersonsAdderService, PersonsAdderService>();
      services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
      services.AddScoped<IPersonsGetterService, PersonsGetterService>();
      services.AddScoped<IPersonsSorterService, PersonsSorterService>();
      services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
      services.AddScoped<ICountriesRepository, CountriesRepositoy>();
      services.AddScoped<IPersonsRepository, PersonsRepository>();

      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
      });
      return services;
    }
  }
}
