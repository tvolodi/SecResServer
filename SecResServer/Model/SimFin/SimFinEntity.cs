using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    /// <summary>
    /// SimFin Entity converted from JSON
    /// </summary>
    public class SimFinEntity : IUpdateTrail
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("simId")]
        public int SimFinId { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }        

        public DateTime LastUpdateDT { get; set; } = DateTime.Now;
        public DateTime? DeleteDT { get; set; } = null;
    }
}
