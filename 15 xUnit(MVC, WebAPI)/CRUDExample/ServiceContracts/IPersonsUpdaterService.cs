using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts
{
  public interface IPersonsUpdaterService
  {
    /// <summary>
    /// Update the specified person details based on the given person ID
    /// </summary>
    /// <param name="personUpdateRequest">Person details to update, including person id</param>
    /// <returns>Returns the person object after updation</returns>
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
  }
}