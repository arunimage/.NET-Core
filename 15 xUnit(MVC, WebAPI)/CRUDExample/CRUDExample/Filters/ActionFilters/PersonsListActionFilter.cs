
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Filters.ActionFilters
{
  public class PersonsListActionFilter : IActionFilter
  {
    private readonly ILogger<PersonsListActionFilter> _logger;
    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
      _logger = logger;
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
      //To do: add after logic here
      _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

      PersonsController personsController = (PersonsController)context.Controller;

      Dictionary<string, object?>? parameters = (Dictionary<string, object?>?)context.HttpContext.Items["arguments"];

      if (parameters != null)
      {
        if (parameters.ContainsKey("searchBy"))
        {
          personsController.ViewData["CurrentSearchBy"] = parameters["searchBy"];
        }

        if (parameters.ContainsKey("searchString"))
        {
          personsController.ViewData["CurrentSearchString"] = parameters["searchString"];
        }

        if (parameters.ContainsKey("sortBy"))
        {
          personsController.ViewData["CurrentSortBy"] = parameters["sortBy"];
        }
        else
        {
          personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
        }

        if (parameters.ContainsKey("sortOrder"))
        {
          personsController.ViewData["CurrentSortOrder"] = parameters["sortOrder"];
        }
        else
        {
          personsController.ViewData["CurrentSortOrder"] = SortOrderOptions.ASC;
        }
      }

      //Searching
      personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(PersonResponse.PersonName) ,"Person Name"},
                {nameof(PersonResponse.Email) ,"Email"},
                {nameof(PersonResponse.DateOfBirth) ,"Date Of Birth"},
                {nameof(PersonResponse.Gender) ,"Gender"},
                {nameof(PersonResponse.CountryID) ,"Country"},
                {nameof(PersonResponse.Address) ,"Address"},
            };
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
      context.HttpContext.Items["arguments"] = context.ActionArguments;
      //To do: add before logic here
      _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

      if (context.ActionArguments.ContainsKey("searchBy"))
      {
        string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

        //validate the searchBy parameter value
        if (!string.IsNullOrEmpty(searchBy))
        {
          var searchByOption = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };

          //reset the searchBy perimeter value
          if (searchByOption.Any(temp => temp == searchBy) == false)
          {
            _logger.LogInformation("searchBy actual value {searchBy}", searchBy);

            context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);

            _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
          }
        }
      }
    }
  }
}
