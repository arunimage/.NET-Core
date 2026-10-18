using Entities;
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
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("776ACCE2-48C1-4E6F-BEB2-0C9D7FDAA480"),
                    PersonName = "Lavena",
                    Email = "lroark0@bandcamp.com",
                    DateOfBirth = new DateTime(1991, 12, 4),
                    Gender = "Female",
                    CountryID = Guid.Parse("8086B03E-F6D7-4915-8B56-75A746A3F83D"),
                    Address = "6 Havey Place",
                    ReceiveNewsLetters = false
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("229D0B9F-3246-451A-8F2D-65F2612AAA86"),
                    PersonName = "Pauly",
                    Email = "pduckwith1@1und1.de",
                    DateOfBirth = new DateTime(1991, 5, 16),
                    Gender = "Female",
                    CountryID = Guid.Parse("8E1883D0-7D83-4482-A8D5-F050D7D5662E"),
                    Address = "77 Armistice Way",
                    ReceiveNewsLetters = true
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("51FD117D-B2A5-42AA-BEBC-35D6B3C93CDD"),
                    PersonName = "Errol",
                    Email = "egarden2@stumbleupon.com",
                    DateOfBirth = new DateTime(2014, 11, 28),
                    Gender = "Male",
                    CountryID = Guid.Parse("C680C5EC-E31F-4488-BFB8-466AD3D7FEF2"),
                    Address = "24655 Rusk Terrace",
                    ReceiveNewsLetters = true
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("975EC117-7086-4558-ABB7-E7060EF89A9D"),
                    PersonName = "Neron",
                    Email = "ntrippett3@redcross.org",
                    DateOfBirth = new DateTime(2023, 4, 13),
                    Gender = "Male",
                    CountryID = Guid.Parse("4EAD9F28-7933-4E14-977A-8B0952217B56"),
                    Address = "50 Waywood Park",
                    ReceiveNewsLetters = true
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("EB70E7BD-9A46-4BAB-9285-5CB61B16D0F7"),
                    PersonName = "Simonette",
                    Email = "sollerhead4@flickr.com",
                    DateOfBirth = new DateTime(2013, 8, 31),
                    Gender = "Female",
                    CountryID = Guid.Parse("371EC99A-D277-4728-8D40-37BAE7D37CD9"),
                    Address = "36 Stephen Crossing",
                    ReceiveNewsLetters = false
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("1F6F775B-2134-40D1-9025-3DD7CF68D0C1"),
                    PersonName = "Guss",
                    Email = "gtomaskunas5@godaddy.com",
                    DateOfBirth = new DateTime(2008, 1, 5),
                    Gender = "Male",
                    CountryID = Guid.Parse("8086B03E-F6D7-4915-8B56-75A746A3F83D"),
                    Address = "49826 Lyons Circle",
                    ReceiveNewsLetters = true
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.NewGuid(),
                    PersonName = "Leonora",
                    Email = "lbenettolo6@cocolog-nifty.com",
                    DateOfBirth = new DateTime(2020, 9, 7),
                    Gender = "Female",
                    CountryID = Guid.Parse("4EAD9F28-7933-4E14-977A-8B0952217B56"),
                    Address = "4845 Mallard Place",
                    ReceiveNewsLetters = true
                });
            }
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();

            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
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
            _persons.Add(person);

            //convert the person object into PersonResponse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
            {
                return null;
            }
            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp => (temp.DateOfBirth != null) ? temp.DateOfBirth.Value.ToString("dd MMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Country) ? temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
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

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)

                throw new ArgumentNullException(nameof(Person));


            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object to update
            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
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

            return ConvertPersonToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person==null)
            {
                return false;
            }

            _persons.RemoveAll(temp => temp.PersonID == personID);

            return true;
        }
    }
}
