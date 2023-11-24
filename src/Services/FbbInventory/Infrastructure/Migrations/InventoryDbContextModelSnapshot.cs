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

                    b.Property<string>("Fulfillability")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LotNumber")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("nvarchar(max)")
                        .HasComputedColumnSql("CONCAT(IIF([Fulfillability] = 'Fulfillable', 'FUFL-', 'UNFL-'), [Id])", true);

                    b.Property<int>("ProductInventoryId")
                        .HasColumnType("int");

                    b.Property<long>("TotalUnits")
                        .HasColumnType("bigint");

                    b.Property<long>("UnitsInStock")
                        .HasColumnType("bigint");

                    b.Property<long>("UnitsPendingRemoval")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProductInventoryId");

                    b.ToTable("Lots");

                    b.HasDiscriminator<string>("Fulfillability").HasValue("Lot");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.ProductInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("HasPickupsInProgress")
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

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.FulfillableLot", b =>
                {
                    b.HasBaseType("Bazaar.FbbInventory.Domain.Entities.Lot");

                    b.Property<DateTime>("DateEnteredStorage")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ProductInventoryId1")
                        .HasColumnType("int");

                    b.HasIndex("ProductInventoryId1");

                    b.HasIndex("ProductInventoryId", "DateEnteredStorage")
                        .IsUnique()
                        .HasFilter("[DateEnteredStorage] IS NOT NULL");

                    b.HasDiscriminator().HasValue("Fulfillable");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.UnfulfillableLot", b =>
                {
                    b.HasBaseType("Bazaar.FbbInventory.Domain.Entities.Lot");

                    b.Property<DateTime>("DateUnfulfillableSince")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductInventoryId2")
                        .HasColumnType("int");

                    b.Property<int>("UnfulfillableCategory")
                        .HasColumnType("int");

                    b.HasIndex("ProductInventoryId2");

                    b.HasIndex("ProductInventoryId", "DateUnfulfillableSince", "UnfulfillableCategory")
                        .IsUnique()
                        .HasFilter("[DateUnfulfillableSince] IS NOT NULL AND [UnfulfillableCategory] IS NOT NULL");

                    b.HasDiscriminator().HasValue("Unfulfillable");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.Lot", b =>
                {
                    b.HasOne("Bazaar.FbbInventory.Domain.Entities.ProductInventory", "ProductInventory")
                        .WithMany()
                        .HasForeignKey("ProductInventoryId")
                        .OnDelete(DeleteBehavior.Restrict)
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

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.FulfillableLot", b =>
                {
                    b.HasOne("Bazaar.FbbInventory.Domain.Entities.ProductInventory", null)
                        .WithMany("FulfillableLots")
                        .HasForeignKey("ProductInventoryId1");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.UnfulfillableLot", b =>
                {
                    b.HasOne("Bazaar.FbbInventory.Domain.Entities.ProductInventory", null)
                        .WithMany("UnfulfillableLots")
                        .HasForeignKey("ProductInventoryId2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.ProductInventory", b =>
                {
                    b.Navigation("FulfillableLots");

                    b.Navigation("UnfulfillableLots");
                });

            modelBuilder.Entity("Bazaar.FbbInventory.Domain.Entities.SellerInventory", b =>
                {
                    b.Navigation("ProductInventories");
                });
#pragma warning restore 612, 618
        }
    }
}
