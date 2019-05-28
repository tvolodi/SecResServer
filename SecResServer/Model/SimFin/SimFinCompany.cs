using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinCompany
    {
        [Key]
        public int Id { get; set; }

        public SimFinEntity SimFinEntity { get; set; }

        [ForeignKey("SimFinEntity")]
        public int SimFinId { get; set; }

        public string Name { get; set; }

        public int PYearEnd { get; set; }

        public int EmployeesQnt { get; set; }

        public SimFinIndustry SimFinIndustry { get; set; }

        [ForeignKey("SimFinIndustry")]
        public int SimFinIndustryId { get; set; }


    }
}
