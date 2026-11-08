using Entities;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services
{
    public class PersonsDeleterService : IPersonsDeleterService
  {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsDeleterService> _logger;

        //constructor
        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsDeleterService> logger)
        {
            _personsRepository = personsRepository;
            _logger = logger;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
            {
                return false;
            }

            await _personsRepository.DeletePersonByPersonID(personID.Value);

            return true;
        }
    }
}
