using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinStdStmt
    {
        [Key]
        public int Id { get; set; }

        public SimFinStmtRegistry GetFinStmtRegistry { get; set; }

        [ForeignKey("GetFinStmtRegistry")]
        public int GetFinStmtRegistryId { get; set; }

        [ForeignKey("SimFinStmtIndustryTemplate")]
        public int SimFinStmtIndustryTemplateId { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public bool IsStmtDetailsLoaded { get; set; }


    }
}
