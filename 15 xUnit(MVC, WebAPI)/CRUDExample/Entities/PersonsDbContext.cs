using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //seed to countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //seed to persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [Dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
        }
    }
}
