using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDExample.Controllers
{

  [Route("[controller]")]
  [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-Key-From-Controller", "My-Value-From-Controller", 3 }, Order = 3)]
  public class PersonsController : Controller
  {
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ILogger<PersonsController> _logger;

    //constructor
    public PersonsController(IPersonsService personsService, ICountriesService countriesService, ILogger<PersonsController> logger)
    {
      _personsService = personsService;
      _countriesService = countriesService;
      _logger = logger;
    }

    //Url: persons/index
    [Route("[action]")]
    [Route("/")]
    [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "MyKey-FromAction", "MyValue-From-Action", 1 }, Order = 1)]

    [TypeFilter(typeof(PersonsListResultFilter))]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
      _logger.LogInformation("Index action method of PersonsController");

      _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");


      //Search
      List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

      //Sort
      List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

      return View(sortedPersons); //Views/Persons/Index.cshtml
    }


    //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
    //Url: persons/create
    [Route("[action]")]
    [HttpGet]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "my-key", "my-value", 4 })]
    public async Task<IActionResult> Create()
    {
      List<CountryResponse> countries = await _countriesService.GetAllCountries();
      ViewBag.Countries = countries.Select(temp =>
        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() }
      );

      //new SelectListItem() { Text="Harsha", Value="1" }
      //<option value="1">Harsha</option>
      return View();
    }


    [HttpPost]
    //Url: persons/create
    [Route("[action]")]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
    public async Task<IActionResult> Create(PersonAddRequest personRequest)
    {
      //call the service method
      PersonResponse personResponse = await _personsService.AddPerson(personRequest);

      //navigate to Index() action method (it makes another get request to "persons/index"
      return RedirectToAction("Index", "Persons");
    }
  }
}