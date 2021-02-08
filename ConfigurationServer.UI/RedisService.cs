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
            var settings = new RedisValue(entity.JsonContent);                        

            await _database.StringSetAsync(key, settings);
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

            return apps.OrderBy(x => x.Name).ToList();
        }

        public async Task<ApplicationEntity> GetApplicationAsync(string name)
        {            
            return new ApplicationEntity
            {
                Name = name,
                JsonContent = _database.StringGet(new RedisKey(name))
            };
        }
    }
}
