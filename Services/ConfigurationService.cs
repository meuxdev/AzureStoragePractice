
using Microsoft.Extensions.Configuration;

namespace AzureStoragePractice.Services
{

    public class ConfigurationService : IConfigurationService
    {
        private string connectionString { get; init; }

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json");

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("AppSettings.json", true, true)
            .Build();

            connectionString = config[nameof(connectionString)];
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

    }


    public interface IConfigurationService
    {
        string GetConnectionString(); // returns the connection string
    }
}