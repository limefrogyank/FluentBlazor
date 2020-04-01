using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace FluentBlazor.Components
{
    public class ReactChild
    {
        public string componentName { get; set; }
        public Dictionary<string, object> parameters { get; set; }
        public IEnumerable<SerializedEvent> serializedEvents { get; set; }
        public List<ReactChild> children { get; set; }

        [JsonIgnore]
        public FluentBase Self { get; set; }

        public ReactChild(FluentBase self)
        {
            Self = self;
            children = new List<ReactChild>();
        }
    }
}
