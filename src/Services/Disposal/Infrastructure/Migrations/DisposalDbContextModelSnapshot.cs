﻿// <auto-generated />
using System;
using Bazaar.Disposal.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Disposal.Infrastructure.Migrations
{
    [DbContext(typeof(DisposalDbContext))]
    partial class DisposalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bazaar.Disposal.Domain.Entities.DisposalOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CancelReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("CreatedByBazaar")
                        .HasColumnType("bit");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DisposalOrders");
                });

            modelBuilder.Entity("Bazaar.Disposal.Domain.Entities.DisposeQuantity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisposalOrderId")
                        .HasColumnType("int");

                    b.Property<string>("InventoryOwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LotNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("UnitsToDispose")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DisposalOrderId");

                    b.HasIndex("LotNumber", "DisposalOrderId")
                        .IsUnique();

                    b.ToTable("DisposeQuantities");
                });

            modelBuilder.Entity("Bazaar.Disposal.Domain.Entities.DisposeQuantity", b =>
                {
                    b.HasOne("Bazaar.Disposal.Domain.Entities.DisposalOrder", "DisposalOrder")
                        .WithMany("DisposeQuantities")
                        .HasForeignKey("DisposalOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DisposalOrder");
                });

            modelBuilder.Entity("Bazaar.Disposal.Domain.Entities.DisposalOrder", b =>
                {
                    b.Navigation("DisposeQuantities");
                });
#pragma warning restore 612, 618
        }
    }
}
