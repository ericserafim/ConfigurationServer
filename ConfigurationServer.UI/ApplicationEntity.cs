using System.Collections.Generic;

namespace ConfigurationServer.UI
{
    public class ApplicationEntity
    {
        public string Name { get; set; }

        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        public string JsonContent { get; set; }
    }
}
