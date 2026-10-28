using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Xunit.Abstractions;

namespace CrudTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personInitialData = new List<Person>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personInitialData);

            _countriesService = new CountriesService(dbContext);
            _personService = new PersonsService(dbContext, _countriesService);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
              {
                  await _personService.AddPerson(personAddRequest);
              });
        }

        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
               {
                   await _personService.AddPerson(personAddRequest);
               });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> person_list = await _personService.GetAllPersons();

            //Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, person_list);
        }
        #endregion


        #region GetPersonByPersonID
        //if we supply null as personID, it should return null response
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(person_response_from_get);
        }

        //If we supply a valid person Id, it should return the valid person details from PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "email@sample.com").Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion


        #region GetAllPersons
        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }

        //GetAllPersons() should return a list of person
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest person_add_request1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "smith@example.com").Create();

            PersonAddRequest person_add_request2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "mary@example.com").Create();

            PersonAddRequest person_add_request3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "rahman@example.com").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_add_request1, person_add_request2, person_add_request3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            foreach (PersonAddRequest temp in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(temp);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetAllPersons();

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_get);
            }
        }
        #endregion


        #region GetFilteredPersons
        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest person_add_request1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "smith@example.com").Create();

            PersonAddRequest person_add_request2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "mary@example.com").Create();

            PersonAddRequest person_add_request3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "rahman@example.com").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_add_request1, person_add_request2, person_add_request3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            foreach (PersonAddRequest temp in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(temp);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> person_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_search);
            }
        }

        //First we will add few persons; and then we will search based on person name with some string. It should return the matching person
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest person_add_request1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_add_request2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "mary@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_add_request3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "rahman@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_add_request1, person_add_request2, person_add_request3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            foreach (PersonAddRequest temp in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(temp);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> person_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, person_list_from_search);
                    }
                }
            }
        }

        #endregion


        #region GetSortedPerson
        //When we sort based on PersonName in DESC, it should return person list in desc on PersonName
        [Fact]
        public async Task GetSortedPerson()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest person_add_request1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_add_request2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "mary@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_add_request3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "rahman@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_add_request1, person_add_request2, person_add_request3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            foreach (PersonAddRequest temp in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(temp);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Act
            List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            }
        }
        #endregion


        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
              {
                  //Act
                  await _personService.UpdatePerson(person_update_request);
              });
        }

        //When we supply invalid PersonID, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>().Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
               {
                   //Act
                   await _personService.UpdatePerson(person_update_request);
               });
        }

        //When PersonName is Null, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.CountryID, country_response.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
              {
                  //Act
                  await _personService.UpdatePerson(person_update_request);
              });
        }

        //First, add a new person and try to update the same
        [Fact]
        public async Task UpdatePerson_PersionFullDetailUpdation()
        {
            //Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.CountryID, country_response.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        }



        #endregion


        #region DeletePerson
        //If you supply valid PersonID, it should return true
        [Fact]
        public async Task DeletrPerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.CountryID, country_response.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            //Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        //If you supply invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }
        #endregion

    }
}

