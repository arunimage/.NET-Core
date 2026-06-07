using ServiceContracts;

namespace Services
{
    public class CitiesService : ICitiesService
    {
        private List<string> _cities;
        private Guid _serviceInstanceID;

        public Guid ServiceInstanceID
        {
            get { return _serviceInstanceID; }
        }
        public CitiesService()
        {
            _serviceInstanceID = Guid.NewGuid();
            _cities = new List<string>()
            {
                "London",
                "Paris",
                "New York",
                "Tokyo",
                "Rome"
            };
        }
        public List<string> GetCities()
        {
            return _cities;
        }
    }
}
