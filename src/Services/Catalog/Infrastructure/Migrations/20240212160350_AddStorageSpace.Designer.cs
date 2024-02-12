﻿// <auto-generated />
using System;
using Bazaar.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20240212160350_AddStorageSpace")]
    partial class AddStorageSpace
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bazaar.Catalog.Domain.Entities.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("AvailableStock")
                        .HasColumnType("bigint");

                    b.Property<int>("FulfillmentMethod")
                        .HasColumnType("int");

                    b.Property<bool>("HasOrdersInProgress")
                        .HasColumnType("bit");

                    b.Property<string>("ImageFilename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ListingStatus")
                        .HasColumnType("int");

                    b.Property<int>("MainDepartmentId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("ProductHeightCm")
                        .HasColumnType("real");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ProductId")
                        .HasComputedColumnSql("CONCAT('PROD-', [Id])", true);

                    b.Property<float>("ProductLengthCm")
                        .HasColumnType("real");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("ProductWidthCm")
                        .HasColumnType("real");

                    b.Property<string>("SellerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SubcategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MainDepartmentId");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.HasIndex("SubcategoryId");

                    b.ToTable("CatalogItems");
                });

            modelBuilder.Entity("Bazaar.Catalog.Domain.Entities.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("Bazaar.Catalog.Domain.Entities.CatalogItem", b =>
                {
                    b.HasOne("Bazaar.Catalog.Domain.Entities.ProductCategory", "MainDepartment")
                        .WithMany("MainDepartmentProducts")
                        .HasForeignKey("MainDepartmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Bazaar.Catalog.Domain.Entities.ProductCategory", "Subcategory")
                        .WithMany("SubcategoryProducts")
                        .HasForeignKey("SubcategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MainDepartment");

                    b.Navigation("Subcategory");
                });

            modelBuilder.Entity("Bazaar.Catalog.Domain.Entities.ProductCategory", b =>
                {
                    b.HasOne("Bazaar.Catalog.Domain.Entities.ProductCategory", "ParentCategory")
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Bazaar.Catalog.Domain.Entities.ProductCategory", b =>
                {
                    b.Navigation("ChildCategories");

                    b.Navigation("MainDepartmentProducts");

                    b.Navigation("SubcategoryProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
