using AutoFixture;
using Moq;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrudTest
{
    public class PersonsControllerTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _countriesServiceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();

            _countriesService = _countriesServiceMock.Object;
            _personsService = _personsServiceMock.Object;
        }

        #region
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonList()
        {

        }
        #endregion
    }
}
