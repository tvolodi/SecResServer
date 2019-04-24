using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.Edgar
{
    public class EdgarDescriptions
    {
        [JsonProperty(PropertyName = "totalrows")]
        public int TotalRows { get; set; }

        [JsonProperty(PropertyName = "descriptions")]
        public List<string> Descriptions { get; set; }
    }
}
