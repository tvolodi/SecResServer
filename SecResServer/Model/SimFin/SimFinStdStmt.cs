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

        public SimFinStmtRegistry SimFinStmtRegistry { get; set; }

        [ForeignKey("SimFinStmtRegistry")]
        public int SimFinStmtRegistryId { get; set; }

        public SimFinStmtIndustryTemplate SimFinStmtIndustryTemplate { get; set; }

        [ForeignKey("SimFinStmtIndustryTemplate")]
        public int SimFinStmtIndustryTemplateId { get; set; }

        public int FYear { get; set; }

        public PeriodType PeriodType { get; set; }

        [ForeignKey("PeriodType")]
        public int PeriodTypeId { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public bool IsStmtDetailsLoaded { get; set; }


    }
}
