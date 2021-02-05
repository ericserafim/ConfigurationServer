using Microsoft.Extensions.Configuration;

namespace ConfigurationServerLib
{
    public static class CsExtentions
    {
        public static IConfigurationBuilder AddRedisConfigServer(
            this IConfigurationBuilder builder,
            string connectionString,
            string appName)
        {
            return builder.Add(new CsJsonConfigurationSource(connectionString, appName));
        }
    }
}
