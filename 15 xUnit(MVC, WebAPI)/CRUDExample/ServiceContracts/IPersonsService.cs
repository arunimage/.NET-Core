using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new person into the existing list of person
        /// </summary>
        /// <param name="personAddRequest"></param>
        /// <returns>Returns the same person details, along with the newly generated PersonID</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);


        /// <summary>
        /// Return all persons
        /// </summary>
        /// <returns>Return a list of objects of PersonResponse type</returns>
        List<PersonResponse> GetAllPersons();


        /// <summary>
        /// Return the person object based on the given person id
        /// </summary>
        /// <param name="personID">Person id to serach</param>
        /// <returns>Returns matching person object</returns>
        PersonResponse? GetPersonByPersonID(Guid? personID);


        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to Search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Return all matching person based on given search field and search string</returns>
        List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
        
        /// <summary>
        /// Returns sorted list of Persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Update the specified person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update, including person id</param>
        /// <returns>Returns the person object after updation</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        
        /// <summary>
        /// Deletes the person based on given person id
        /// </summary>
        /// <param name="personID">PersonID to Delete</param>
        /// <returns>True if a person was deleted; otherwise, false.</returns>
        bool DeletePerson(Guid? personID);
    }
}
