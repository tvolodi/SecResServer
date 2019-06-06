using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinStdStmtDetail
    {
        [Key]
        public int Id { get; set; }

        public SimFinStdStmt SimFinStdStmt { get; set; }

        [ForeignKey("SimFinStdStmt")]
        public int SimFinStdStmtId { get; set; }

        public SimFinStmtDetailType SimFinStmtDetailType { get; set; }

        [ForeignKey("SimFinStmtDetailType")]
        public int SimFinStmtDetailTypeId { get; set; }

        public int TId { get; set; }

        public int ParentTId { get; set; }

        public double Value { get; set; }

    }
}
