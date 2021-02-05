using Microsoft.Extensions.Configuration;

namespace ConfigurationServerLib
{
    public class CsJsonConfigurationSource : IConfigurationSource
    {
        private readonly string _connectionString;
        private readonly string _appName;
        private readonly string _keyField;

        public CsJsonConfigurationSource(string connectionString, string appName, string keyField)
        {
            _connectionString = connectionString;
            _appName = appName;
            _keyField = keyField;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CsJsonConfigurationProvider(_connectionString, _appName, _keyField);
        }
    }
}
