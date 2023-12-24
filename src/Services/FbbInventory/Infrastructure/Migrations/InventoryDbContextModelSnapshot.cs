﻿// <auto-generated />
using System;
using Bazaar.FbbInventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    [DbContext(typeof(FbbInventoryDbContext))]
    partial class InventoryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.Lot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DateUnitsBecameStranded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUnitsBecameUnfulfillable")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUnitsEnteredStorage")
                        .HasColumnType("datetime2");

                    b.Property<string>("LotNumber")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("nvarchar(450)")
                        .HasComputedColumnSql("CONCAT('LOT-', [Id])");

                    b.Property<int>("ProductInventoryId")
                        .HasColumnType("int");

                    b.Property<int?>("UnfulfillableCategory")
                        .HasColumnType("int");

                    b.Property<long>("UnitsInRemoval")
                        .HasColumnType("bigint");

                    b.Property<long>("UnitsInStock")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("LotNumber")
                        .IsUnique();

                    b.HasIndex("ProductInventoryId", "DateUnitsEnteredStorage", "DateUnitsBecameStranded", "DateUnitsBecameUnfulfillable")
                        .IsUnique()
                        .HasFilter("[DateUnitsBecameStranded] IS NOT NULL AND [DateUnitsBecameUnfulfillable] IS NOT NULL");

                    b.ToTable("Lots");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.ProductInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("HasPickupsInProgress")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStranded")
                        .HasColumnType("bit");

                    b.Property<long>("MaxStockThreshold")
                        .HasColumnType("bigint");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("RestockThreshold")
                        .HasColumnType("bigint");

                    b.Property<int>("SellerInventoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.HasIndex("SellerInventoryId");

                    b.ToTable("ProductInventories");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.SellerInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("SellerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SellerId")
                        .IsUnique();

                    b.ToTable("SellerInventories");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.Lot", b =>
                {
                    b.HasOne("Bazaar.FbbInventory.Domain.Entities.ProductInventory", "ProductInventory")
                        .WithMany("Lots")
                        .HasForeignKey("ProductInventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductInventory");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.ProductInventory", b =>
                {
                    b.HasOne("Bazaar.FbbInventory.Domain.Entities.SellerInventory", "SellerInventory")
                        .WithMany("ProductInventories")
                        .HasForeignKey("SellerInventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SellerInventory");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.ProductInventory", b =>
                {
                    b.Navigation("Lots");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.SellerInventory", b =>
                {
                    b.Navigation("ProductInventories");
                });
#pragma warning restore 612, 618
        }
    }
}
