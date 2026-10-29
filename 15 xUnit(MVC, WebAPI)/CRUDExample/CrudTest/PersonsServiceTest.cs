using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Xunit.Abstractions;
using FluentAssertions;
using AutoFixture.Kernel;
using System.Linq.Expressions;

namespace CrudTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            _personService = new PersonsService(_personsRepository);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            Person person = personAddRequest.ToPerson();

            //When PersonRepository.AddPerson is called , it has to return the same "person" object
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //If we supply any argument value to the AddPerson method it should return the same return value
            _personsRepositoryMock.Setup
                (temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            //Assert
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);
        }
        #endregion


        #region GetPersonByPersonID
        //if we supply null as personID, it should return null response
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
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
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@sample.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person.PersonID);

            //Assert
            Assert.Equal(person_response_expected, person_response_from_get);
        }
        #endregion


        #region GetAllPersons
        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }

        //First we add few persons; and then we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>(){
                _fixture.Build<Person>().With(temp => temp.Email, "smith@example.com").With(temp=>temp.Country, null as Country).Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "mary@example.com").With(temp=>temp.Country, null as Country).Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "rahman@example.com").With(temp=>temp.Country, null as Country).Create()
            };


            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetAllPersons();

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                Assert.Contains(person_response_from_add, person_list_from_get);
            }
        }
        #endregion


        #region GetFilteredPersons
        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
             .ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        //First we will add few persons; and then we will search based on person name with some string. It should return the matching person
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
             .ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion


        #region GetSortedPerson
        //When we sort based on PersonName in DESC, it should return person list in desc on PersonName
        [Fact]
        public async Task GetSortedPerson_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
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

            person_response_list_expected = person_response_list_expected.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_list_expected.Count; i++)
            {
                Assert.Equal(person_response_list_expected[i], person_list_from_sort[i]);
            }
        }
        #endregion


        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
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
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange            
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, GenderOptions.Male.ToString)
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
              {
                  //Act
                  await _personService.UpdatePerson(person_update_request);
              });
        }

        //First, add a new person and try to update the same
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "smith@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, GenderOptions.Female.ToString)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            //Assert
            Assert.Equal(person_response_expected, person_response_from_update);
        }



        #endregion


        #region DeletePerson
        //If you supply valid PersonID, it should return true
        [Fact]
        public async Task DeletrPerson_ValidPersonID()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "mary@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, GenderOptions.Female.ToString)
                .Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted = await _personService.DeletePerson(person.PersonID);

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

