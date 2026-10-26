using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represent business logic for manupulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it<including newly gernerated Country id/returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the lis
        /// </summary>
        /// <returns>All countries from the list as list of CountryRespne</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Return a country object based on given Country Id
        /// </summary>
        /// <param name="countryID">CountryID (guid) to search</param>
        /// <returns>Matching Country as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}