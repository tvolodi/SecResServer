using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinOrigStmtDetail
    {
        [Key]
        public int Id { get; set; }

        public int LineItemId { get; set; }

        public StmtDetailName StmtDetailName { get; set; }

        [ForeignKey("StmtDetailName")]
        public int StmtDetailNameId { get; set; }

        public SimFinOriginalStmt SimFinOriginalStmt { get; set; }

        [ForeignKey("SimFinOriginalStmt")]
        public int SimFinOriginalStmtId { get; set; }

        public double Value { get; set; }
    }
}
