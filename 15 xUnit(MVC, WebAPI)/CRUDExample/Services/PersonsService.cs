using Entities;
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
    public class PersonsService : IPersonsService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsService> _logger;

        //constructor
        public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService> logger)
        {
            _personsRepository = personsRepository;
            _logger = logger;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {

            //check if PersonAddRequest is not null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            //add person object to person list
            await _personsRepository.AddPerson(person);

            //From Store Procedure
            //_db.sp_InsertPerson(person);

            //convert the person object into PersonResponse type
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonService");

            //SELECT * from Persons
            var persons = await _personsRepository.GetAllPersons();

            return persons.Select(temp =>
                temp.ToPersonResponse()).ToList();

            //Using Store procedure
            //return _db.sp_GetAllPersons()
            //    .Select(temp =>
            //    temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
            {
                return null;
            }
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonService");

            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
               await _personsRepository.GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
             await _personsRepository.GetFilteredPersons(temp => temp.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                   await _personsRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                nameof(PersonResponse.Gender) =>
              await _personsRepository.GetFilteredPersons(temp => temp.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryID) =>
                      await _personsRepository.GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) =>
             await _personsRepository.GetFilteredPersons(temp => temp.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
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
                throw new ArgumentException("Given person id doesn't exist");
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

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
            {
                return false;
            }

            await _personsRepository.DeletePersonByPersonID(personID.Value);

            return true;
        }
    }
}
