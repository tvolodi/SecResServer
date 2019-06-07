using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinOriginalStmt
    {
        [Key]
        public int Id { get; set; }

        public SimFinStmtRegistry SimFinStmtRegistry { get; set; }

        [ForeignKey("SimFinStmtRegistry")]
        public int SimFinStmtRegistryId { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public DateTime FirstPublishedDate { get; set; }

        public Currency Currency { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        public PeriodType PeriodType { get; set; }

        [ForeignKey("PeriodType")]
        public int PeriodTypeId { get; set; }

        public int FYear { get; set; }

        public bool IsStmtDetailsLoaded { get; set; }
    }
}
