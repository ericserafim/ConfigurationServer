using Microsoft.Extensions.Configuration;

namespace ConfigurationServerLib
{
    public static class CsExtentions
    {
        public static IConfigurationBuilder AddRedisConfigServer(
            this IConfigurationBuilder builder,
            string keyField = "name")
        {
            var configRoot = builder.Build();

            return builder.Add(new CsJsonConfigurationSource(
                configRoot["ConfigServer:ConnectionString"], 
                configRoot["ConfigServer:AppName"], 
                keyField));
        }

        public static IConfigurationBuilder AddRedisConfigServer(
           this IConfigurationBuilder builder,
           string connectionString,
           string appName,
           string keyField = "name")
        { 
            return builder.Add(new CsJsonConfigurationSource(connectionString, appName, keyField));
        }
    }
}
