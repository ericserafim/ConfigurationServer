using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationServer.UI
{
    public class RedisService
    {
        private readonly IDatabase _database;
        private readonly IServer _server;
        private readonly ISubscriber _subscriber;

        public RedisService(IOptionsSnapshot<RedisSettings> settings)
        {
            var connection = ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
            _subscriber = connection.GetSubscriber();
            _database = connection.GetDatabase();
            _server = connection.GetServer(connection.GetEndPoints().First());
        }

        public async Task SaveApplicationAsync(ApplicationEntity entity)
        {
            var key = new RedisKey(entity.Name);
            var hashSettings = entity.Settings.Select(x => new HashEntry(x.Key, x.Value)).ToArray();

            //Handle with removed hash sets
            await _database.KeyDeleteAsync(key);

            await _database.HashSetAsync(key, hashSettings);
        }

        public async Task PublishChangesAsync(string applicationName)
        {
            await _subscriber.PublishAsync(new RedisChannel(applicationName, RedisChannel.PatternMode.Literal), $"{applicationName} settings has been updated");
        }

        public async Task RemoveApplicationAsync(ApplicationEntity entity)
        {
            var key = new RedisKey(entity.Name);
            await _database.KeyDeleteAsync(key);
        }

        public async Task<List<ApplicationEntity>> GetAllApplicationsAsync()
        {
            var apps = new List<ApplicationEntity>();

            await foreach (var key in _server.KeysAsync())
            {
                apps.Add(new ApplicationEntity { Name = key.ToString() });
            }

            return apps;
        }

        public async Task<ApplicationEntity> GetApplicationAsync(string name)
        {
            var hashSet = await _database.HashGetAllAsync(new RedisKey(name));

            return new ApplicationEntity
            {
                Name = name,
                Settings = hashSet
                 .Select(x => new KeyValuePair<string, string>(x.Name, x.Value))
                 .ToDictionary(k => k.Key, v => v.Value)
            };
        }
    }
}
