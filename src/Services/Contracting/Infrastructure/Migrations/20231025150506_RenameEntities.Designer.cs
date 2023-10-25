﻿// <auto-generated />
using System;
using Bazaar.Contracting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bazaar.Contracting.Infrastructure.Migrations
{
    [DbContext(typeof(ContractingDbContext))]
    [Migration("20231025150506_RenameEntities")]
    partial class RenameEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bazaar.Contracting.Domain.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ExternalId")
                        .HasComputedColumnSql("CONCAT('PNER-', [Id])", true);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Clients", t =>
                        {
                            t.HasCheckConstraint("CK_Client_18AndOlder", "DATEDIFF(year, [DateOfBirth], CAST(GETDATE() as date)) >= 18 AND [DateOfBirth] < GETDATE()");
                        });
                });

            modelBuilder.Entity("Bazaar.Contracting.Domain.Entities.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("SellingPlanId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("SellingPlanId");

                    b.ToTable("Contracts", t =>
                        {
                            t.HasCheckConstraint("CK_Contract_StartBeforeEndDate", "[EndDate] IS NULL OR [StartDate] <= [EndDate]");

                            t.HasCheckConstraint("CK_Contract_StartDateFromToday", "[StartDate] >= CAST(GETDATE() as date)");
                        });
                });

            modelBuilder.Entity("Bazaar.Contracting.Domain.Entities.SellingPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("MonthlyFee")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PerSaleFee")
                        .HasColumnType("decimal(18,2)");

                    b.Property<float>("RegularPerSaleFeePercent")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("SellingPlans", t =>
                        {
                            t.HasCheckConstraint("CK_SellingPlan_PerSaleOrMonthlyFeePositive", "[PerSaleFee] > 0 OR [MonthlyFee] > 0");

                            t.HasCheckConstraint("CK_SellingPlan_RegularFeePositive", "[RegularPerSaleFeePercent] > 0");
                        });
                });

            modelBuilder.Entity("Bazaar.Contracting.Domain.Entities.Contract", b =>
                {
                    b.HasOne("Bazaar.Contracting.Domain.Entities.Client", "Client")
                        .WithMany("Contracts")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bazaar.Contracting.Domain.Entities.SellingPlan", "SellingPlan")
                        .WithMany()
                        .HasForeignKey("SellingPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("SellingPlan");
                });

            modelBuilder.Entity("Bazaar.Contracting.Domain.Entities.Client", b =>
                {
                    b.Navigation("Contracts");
                });
#pragma warning restore 612, 618
        }
    }
}
