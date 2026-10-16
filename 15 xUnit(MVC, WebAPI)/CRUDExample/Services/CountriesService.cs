using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        private readonly List<Country> _countries;

        //constructor
        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>() {
                new Country() { CountryID = Guid.Parse("8086B03E-F6D7-4915-8B56-75A746A3F83D"), CountryName = "USA" },
                new Country() { CountryID = Guid.Parse("8E1883D0-7D83-4482-A8D5-F050D7D5662E"), CountryName = "Canada" },
                new Country() { CountryID = Guid.Parse("C680C5EC-E31F-4488-BFB8-466AD3D7FEF2"), CountryName = "UK" },
                new Country() { CountryID = Guid.Parse("4EAD9F28-7933-4E14-977A-8B0952217B56"), CountryName = "India" },
                new Country() { CountryID = Guid.Parse("371EC99A-D277-4728-8D40-37BAE7D37CD9"), CountryName = "Australia" }
                });
            }
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: CountryAddRequestParameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (_countries.Where(temp =>
            temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given CountryName is Already Present");
            }

            //Convert Object from CountryAddResquest to Country type
            Country country = countryAddRequest.ToCountry();

            //Generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add Country object into _countries
            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }
            Country? country_response_from_list = _countries.FirstOrDefault(temp => temp.CountryID == countryID);


            if (country_response_from_list == null)
            {
                return null;
            }

            return country_response_from_list.ToCountryResponse();
        }
    }
}
