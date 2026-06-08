using ServiceContracts.DTO;
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
    }
}
