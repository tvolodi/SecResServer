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

        public DbSet<PeriodType> PeriodTypes { get; set; }
        public DbSet<StmtType> StmtTypes { get; set; }

        public DbSet<EdgarCompany> EdgarCompanies { get; set; }
        
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<StmtDetailName> StmtDetailNames { get; set; }

        // SimFin Domain

        public DbSet<SimFinEntity> SimFinEntities { get; set; }

        public DbSet<SimFinEntityProgress> SimFinEntityProgresses { get; set; }

        public DbSet<SimFinIndustry> SimFinIndustries { get; set; }

        public DbSet<SimFinOriginalStmt> SimFinOriginalStmts { get; set; }

        public DbSet<SimFinOrigStmtDetail> SimFinOrigStmtDetails { get; set; }

        public DbSet<SimFinStdStmt> SimFinStdStmts { get; set; }

        public DbSet<SimFinStdStmtDetail> SimFinStdStmtDetails { get; set; }

        public DbSet<SimFinStmtDetailType> SimFinStmtDetailTypes { get; set; }

        public DbSet<SimFinStmtRegistry> SimFinStmtRegistries { get; set; }

        public DbSet<SimFinRequestLog> SimFinRequestLogs { get; set; }

        public DbSet<SimFinSector> SimFinSectors { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasIndex(c => c.CharCode);
            modelBuilder.Entity<Currency>()
                .HasIndex(c => c.Id);
        }


    }
}
