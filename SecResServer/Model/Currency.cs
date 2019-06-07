using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model
{
    public class Currency
    {       
        public int Id { get; set; }

        public string CharCode { get; set; }

        public string Name { get; set; }

    }
}
