using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class StmtListJsonConvEntity
    {
        public Dictionary<string, List<StmtEntity>> stmtDict { get; set; }

        //[JsonProperty("pl")]
        //public Dictionary<string, Dictionary<int, StmtEntity>> plStmts { get; set; }

        //[JsonProperty("bs")]
        //public Dictionary<string, Dictionary<int, StmtEntity>> bsStmts { get; set; }

        //[JsonProperty("cf")]
        //public Dictionary<string, Dictionary<int, StmtEntity>> cfStmts { get; set; }

    }

    public class StmtEntity
    {
        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("fyear")]
        public int FYear { get; set; }

        [JsonProperty("calculated")]
        public bool IsCalculated { get; set; }
    }
}
