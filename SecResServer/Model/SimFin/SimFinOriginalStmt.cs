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

        [ForeignKey("GetFinStmtRegistry")]
        public int GetFinStmtRegistryId { get; set; }

        public bool IsStmtDetailsLoaded { get; set; }

    }
}
