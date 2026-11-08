using Entities;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services
{
  public class PersonsSorterService : IPersonsSorterService
  {
    //private field
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsSorterService> _logger;

    //constructor
    public PersonsSorterService(IPersonsRepository personsRepository, ILogger<PersonsSorterService> logger)
    {
      _personsRepository = personsRepository;
      _logger = logger;
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
      _logger.LogInformation("GetSortedPersons of PersonService");

      if (string.IsNullOrEmpty(sortBy))
      {
        return allPersons;
      }

      List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
      {
        (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

        _ => allPersons
      };
      return sortedPersons;
    }

  }
}
