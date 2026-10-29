using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add a new person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the data store</returns>
        Task<Person> AddPerson(Person person);


        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>All persons from the table</returns>
        Task<List<Person>> GetAllPersons();


        /// <summary>
        /// Return a person object based on the given person id; otherwise, it returns null
        /// </summary>
        /// <param name="personID">PersonID to search</param>
        /// <returns>Matching person or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);


        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);


        /// <summary>
        /// Deletes a person object based on the person id
        /// </summary>
        /// <param name="personID">Person ID (guid) to search</param>
        /// <returns>return true if the deletion is successful</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);


        /// <summary>
        /// Updates a person object based on the given person id
        /// </summary>
        /// <param name="person">person object to update</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
