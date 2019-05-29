﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model.SimFin
{
    public class SimFinEntityProgress
    {
        [Key]
        public int Id { get; set; }

        public int SimFinEntityId { get; set; }

        public bool IsCompanyDataLoaded { get; set; } = false;

        public bool IsInitStmtLoaded { get; set; }
    }
}
