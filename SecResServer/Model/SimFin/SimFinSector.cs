using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinSector
    {
        [Key]
        public int Id { get; set; }

        public int Code { get; set; }

        public string Name { get; set; }
    }
}
