﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SecResServer.Model;

namespace SecResServer.Migrations
{
    [DbContext(typeof(SecResDbContext))]
    [Migration("20190607033829_SimFinCurr2")]
    partial class SimFinCurr2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SecResServer.Model.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CountryId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("SecResServer.Model.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("SecResServer.Model.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CharCode");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CharCode");

                    b.HasIndex("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("SecResServer.Model.EdgarCompany", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Cik");

                    b.Property<DateTime?>("DeleteDT");

                    b.Property<int>("EntityId");

                    b.Property<DateTime>("LastUpdateDT");

                    b.Property<string>("MarketOperator");

                    b.Property<string>("Markettier");

                    b.Property<string>("Name");

                    b.Property<string>("PrimaryExchange");

                    b.Property<string>("PrimarySymbol");

                    b.Property<int>("SicCode");

                    b.Property<string>("SicDescription");

                    b.HasKey("Id");

                    b.ToTable("EdgarCompanies");
                });

            modelBuilder.Entity("SecResServer.Model.PeriodType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("PeriodTypes");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeleteDT");

                    b.Property<DateTime>("LastUpdateDT");

                    b.Property<string>("Name");

                    b.Property<int>("SimFinId");

                    b.Property<string>("Ticker");

                    b.HasKey("Id");

                    b.ToTable("SimFinEntities");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinEntityProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsCompanyDataLoaded");

                    b.Property<bool>("IsInitStmtLoaded");

                    b.Property<int>("SimFinEntityId");

                    b.HasKey("Id");

                    b.ToTable("SimFinEntityProgresses");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinIndustry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Code");

                    b.Property<string>("Name");

                    b.Property<int>("SimFinSectorId");

                    b.HasKey("Id");

                    b.HasIndex("SimFinSectorId");

                    b.ToTable("SimFinIndustries");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinOriginalStmt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrencyId");

                    b.Property<int>("FYear");

                    b.Property<DateTime>("FirstPublishedDate");

                    b.Property<bool>("IsStmtDetailsLoaded");

                    b.Property<DateTime>("PeriodEndDate");

                    b.Property<int>("PeriodTypeId");

                    b.Property<int>("SimFinStmtRegistryId");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("PeriodTypeId");

                    b.HasIndex("SimFinStmtRegistryId");

                    b.ToTable("SimFinOriginalStmts");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinRequestLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("RequestDT");

                    b.HasKey("Id");

                    b.ToTable("SimFinRequestLogs");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinSector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Code");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("SimFinSectors");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStdStmt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GetFinStmtRegistryId");

                    b.Property<bool>("IsStmtDetailsLoaded");

                    b.Property<DateTime>("PeriodEndDate");

                    b.Property<int>("SimFinStmtIndustryTemplateId");

                    b.HasKey("Id");

                    b.HasIndex("GetFinStmtRegistryId");

                    b.ToTable("SimFinStdStmts");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStdStmtDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ParentTId");

                    b.Property<int>("SimFinStdStmtId");

                    b.Property<int>("SimFinStmtDetailTypeId");

                    b.Property<int>("TId");

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.HasIndex("SimFinStdStmtId");

                    b.HasIndex("SimFinStmtDetailTypeId");

                    b.ToTable("SimFinStdStmtDetails");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStmtDetailType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("SimFinStmtDetailTypes");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStmtRegistry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FYear");

                    b.Property<bool>("IsCalculated");

                    b.Property<bool>("IsStmtLoaded");

                    b.Property<DateTime>("LoadDateTime");

                    b.Property<int>("PeriodTypeId");

                    b.Property<int>("SimFinEntityId");

                    b.Property<int>("StmtTypeId");

                    b.HasKey("Id");

                    b.HasIndex("PeriodTypeId");

                    b.HasIndex("SimFinEntityId");

                    b.HasIndex("StmtTypeId");

                    b.ToTable("SimFinStmtRegistries");
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.StmtType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("StmtTypes");
                });

            modelBuilder.Entity("SecResServer.Model.Company", b =>
                {
                    b.HasOne("SecResServer.Model.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinIndustry", b =>
                {
                    b.HasOne("SecResServer.Model.SimFin.SimFinSector", "SimFinSector")
                        .WithMany()
                        .HasForeignKey("SimFinSectorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinOriginalStmt", b =>
                {
                    b.HasOne("SecResServer.Model.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SecResServer.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SecResServer.Model.SimFin.SimFinStmtRegistry", "SimFinStmtRegistry")
                        .WithMany()
                        .HasForeignKey("SimFinStmtRegistryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStdStmt", b =>
                {
                    b.HasOne("SecResServer.Model.SimFin.SimFinStmtRegistry", "GetFinStmtRegistry")
                        .WithMany()
                        .HasForeignKey("GetFinStmtRegistryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStdStmtDetail", b =>
                {
                    b.HasOne("SecResServer.Model.SimFin.SimFinStdStmt", "SimFinStdStmt")
                        .WithMany()
                        .HasForeignKey("SimFinStdStmtId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SecResServer.Model.SimFin.SimFinStmtDetailType", "SimFinStmtDetailType")
                        .WithMany()
                        .HasForeignKey("SimFinStmtDetailTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SecResServer.Model.SimFin.SimFinStmtRegistry", b =>
                {
                    b.HasOne("SecResServer.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SecResServer.Model.SimFin.SimFinEntity", "SimFinEntity")
                        .WithMany()
                        .HasForeignKey("SimFinEntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SecResServer.Model.SimFin.StmtType", "StmtType")
                        .WithMany()
                        .HasForeignKey("StmtTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
