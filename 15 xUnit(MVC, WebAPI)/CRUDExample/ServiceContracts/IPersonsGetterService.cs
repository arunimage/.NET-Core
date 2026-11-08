using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Return all persons
        /// </summary>
        /// <returns>Return a list of objects of PersonResponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();


        /// <summary>
        /// Return the person object based on the given person id
        /// </summary>
        /// <param name="personID">Person id to serach</param>
        /// <returns>Returns matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);


        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to Search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Return all matching person based on given search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
    }
}