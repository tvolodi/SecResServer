using Microsoft.EntityFrameworkCore;
using SecResServer.Model.SimFin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace SecResServer.Model
{
    public class SecResDbContext : DbContext
    {
        private string connectionString = string.Empty;

        public SecResDbContext(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public SecResDbContext(DbContextOptions<SecResDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {             
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        public DbSet<EdgarCompany> EdgarCompanies { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        
        // SimFin Domain
        public DbSet<SimFinEntity> simFinEntities { get; set; }


    }
}
