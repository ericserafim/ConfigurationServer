using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurationServerLib
{
    public class CsJsonConfigurationProvider : ConfigurationProvider
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly ISubscriber _subscriber;
        private readonly string _appName;

        public CsJsonConfigurationProvider(string connectionSttring, string appName)
        {
            _appName = appName;
            _connection = ConnectionMultiplexer.Connect(connectionSttring);
            _subscriber = _connection.GetSubscriber();

            _subscriber.Subscribe(
                new RedisChannel(_appName, RedisChannel.PatternMode.Literal), 
                handler: (channel, value) => 
                {                    
                    Console.WriteLine(value);
                    Load();
                });

        }

        public override void Load()
        {
            var db = _connection.GetDatabase();
            var settings = db.HashGetAll(new RedisKey(_appName));

            Data = settings
                .Select(x => new KeyValuePair<string, string>(x.Name, x.Value))
                .ToDictionary(k => k.Key, v => v.Value);
        }
    }
}