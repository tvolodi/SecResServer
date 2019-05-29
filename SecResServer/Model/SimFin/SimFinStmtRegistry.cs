using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinStmtRegistry
    {
        [Key]
        public int Id { get; set; }

        public SimFinEntity SimFinEntity { get; set; }

        [ForeignKey("SimFinEntity")]
        public int SimFinEntityId { get; set; }

        public StmtType StmtType { get; set; }

        [ForeignKey("StmtType")]
        public int StmtTypeId { get; set; }

        public int FYear { get; set; }

        public PeriodType PeriodType { get; set; }

        [ForeignKey("PeriodType")]
        public int PeriodTypeId { get; set; }

        public DateTime LoadDateTime { get; set; }

        public bool IsCalculated { get; set; }


    }
}
