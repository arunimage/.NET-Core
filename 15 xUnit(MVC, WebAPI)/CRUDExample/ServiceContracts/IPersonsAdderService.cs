using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
    public interface IPersonsAdderService
  {
        /// <summary>
        /// Adds a new person into the existing list of person
        /// </summary>
        /// <param name="personAddRequest"></param>
        /// <returns>Returns the same person details, along with the newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
        }
}