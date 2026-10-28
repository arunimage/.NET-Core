using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);


        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAllCountries();


        /// <summary>
        /// Return a country object based on the given country id; otherwise, it returns null
        /// </summary>
        /// <param name="countryID">CountryID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);


        /// <summary>
        /// Returns a country based on the given country name
        /// </summary>
        /// <param name="countryName">Country name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
