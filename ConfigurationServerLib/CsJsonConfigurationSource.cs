using Microsoft.Extensions.Configuration;

namespace ConfigurationServerLib
{
    public class CsJsonConfigurationSource : IConfigurationSource
    {
        private readonly string _connectionString;
        private readonly string _appName;

        public CsJsonConfigurationSource(string connectionString, string appName)
        {
            _connectionString = connectionString;
            _appName = appName;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CsJsonConfigurationProvider(_connectionString, _appName);
        }
    }
}
