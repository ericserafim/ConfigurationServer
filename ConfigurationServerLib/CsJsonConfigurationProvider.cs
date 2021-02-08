using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConfigurationServerLib
{
    public class CsJsonConfigurationProvider : ConfigurationProvider
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly ISubscriber _subscriber;
        private readonly string _appName;
        private readonly string _keyField;

        public CsJsonConfigurationProvider(string connectionSttring, string appName, string keyField)
        {
            _appName = appName;
            _keyField = keyField;

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
            var settingsJson = db.StringGet(new RedisKey(_appName));

            if (string.IsNullOrEmpty(settingsJson))
                return;

            using(var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(settingsJson.ToString())))
            {
                Data = JsonConfigurationFileParserCustom.Parse(memoryStream, _keyField);
            }
        }
    }
}