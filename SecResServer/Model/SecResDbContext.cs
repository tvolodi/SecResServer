using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Model
{
    public class SecResDbContext : DbContext
    {
        public SecResDbContext(DbContextOptions<SecResDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        



    }
}
