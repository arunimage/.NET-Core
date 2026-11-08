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
  public class PersonsUpdaterService : IPersonsUpdaterService
  {
    //private field
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsUpdaterService> _logger;

    //constructor
    public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger)
    {
      _personsRepository = personsRepository;
      _logger = logger;
    }
    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
      if (personUpdateRequest == null)

        throw new ArgumentNullException(nameof(Person));


      //validation
      ValidationHelper.ModelValidation(personUpdateRequest);

      //get matching person object to update
      Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
      if (matchingPerson == null)
      {
        throw new InvalidPersonIDException("Given person id doesn't exist");
      }

      //update all details
      matchingPerson.PersonName = personUpdateRequest.PersonName;
      matchingPerson.Email = personUpdateRequest.Email;
      matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
      matchingPerson.Gender = personUpdateRequest.Gender.ToString();
      matchingPerson.CountryID = personUpdateRequest.CountryID;
      matchingPerson.Address = personUpdateRequest.Address;
      matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

      await _personsRepository.UpdatePerson(matchingPerson);      //Update

      return matchingPerson.ToPersonResponse();
    }
  }
}