﻿// <auto-generated />
using System;
using Bazaar.Contracting.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Contracting.Migrations
{
    [DbContext(typeof(ContractingDbContext))]
    [Migration("20230911163622_CorrectEndDateAllowNull")]
    partial class CorrectEndDateAllowNull
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bazaar.Contracting.Core.Model.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.Property<int>("SellingPlanId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("PartnerId");

                    b.HasIndex("SellingPlanId");

                    b.ToTable("Contracts", t =>
                        {
                            t.HasCheckConstraint("CK_Contract_StartBeforeEndDate", "[EndDate] IS NULL OR [StartDate] <= [EndDate]");

                            t.HasCheckConstraint("CK_Contract_StartDateFromToday", "[StartDate] >= CAST(GETDATE() as date)");
                        });
                });

            modelBuilder.Entity("Bazaar.Contracting.Core.Model.Partner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Partners", t =>
                        {
                            t.HasCheckConstraint("CK_Partner_18AndOlder", "DATEDIFF(year, [DateOfBirth], CAST(GETDATE() as date)) >= 18 AND [DateOfBirth] < GETDATE()");
                        });
                });

            modelBuilder.Entity("Bazaar.Contracting.Core.Model.SellingPlan", b =>
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

                    b.ToTable("SellingPlans");
                });

            modelBuilder.Entity("Bazaar.Contracting.Core.Model.Contract", b =>
                {
                    b.HasOne("Bazaar.Contracting.Core.Model.Partner", "Partner")
                        .WithMany("Contracts")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bazaar.Contracting.Core.Model.SellingPlan", "SellingPlan")
                        .WithMany()
                        .HasForeignKey("SellingPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partner");

                    b.Navigation("SellingPlan");
                });

            modelBuilder.Entity("Bazaar.Contracting.Core.Model.Partner", b =>
                {
                    b.Navigation("Contracts");
                });
#pragma warning restore 612, 618
        }
    }
}