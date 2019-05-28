using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinIndustry
    {
        [Key]
        public int Id { get; set; }

        public int Code { get; set; }

        public string Name { get; set; }

        public SimFinSector SimFinSector { get; set; }

        [ForeignKey("SimFinSector")]
        public int SimFinSectorId { get; set; }
    }
}
