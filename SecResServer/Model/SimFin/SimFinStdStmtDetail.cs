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

        public StmtDetailName StmtDetailName { get; set; }

        [ForeignKey("StmtDetailName")]
        public int StmtDetailNameId { get; set; }

        public int TId { get; set; }

        public int UId { get; set; }

        public int DisplayLevel { get; set; }

        public int ParentTId { get; set; }

        public double ValueAssigned { get; set; }

        public double ValueCalculated { get; set; }

        public double ValueChosen { get; set; }

    }
}
