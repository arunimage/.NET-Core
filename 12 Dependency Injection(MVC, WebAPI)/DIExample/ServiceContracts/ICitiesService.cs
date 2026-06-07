namespace ServiceContracts
{
    public interface ICitiesService
    {
        List<string> GetCities();

        Guid ServiceInstanceID { get; }
    }
}
