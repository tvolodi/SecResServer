using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model
{  
    public class EdgarCompany
    {
        [Key]
        public int Id { get; set; }

        public string Cik { get; set; }

        public string Name { get; set; }

        public int EntityId { get; set; }

        public string PrimaryExchange { get; set; }

        public string MarketOperator { get; set; }

        public string Markettier { get; set; }

        public string PrimarySymbol { get; set; }

        public int SicCode { get; set; }

        public string SicDescription { get; set; }


    }
}
