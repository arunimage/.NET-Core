using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
    public interface IPersonsDeleterService
  {
        /// <summary>
        /// Deletes the person based on given person id
        /// </summary>
        /// <param name="personID">PersonID to Delete</param>
        /// <returns>True if a person was deleted; otherwise, false.</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}