﻿// <auto-generated />
using DBAplication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DBAplication.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DBAplication.Model.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("KeyAuthorized");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("DBAplication.Model.Order", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<string>("CurrentStatus");

                    b.Property<string>("Date");

                    b.Property<string>("FromAddress");

                    b.Property<string>("Milisse");

                    b.Property<string>("NameCustomer");

                    b.Property<string>("NoName");

                    b.Property<string>("NoName1");

                    b.Property<string>("NoName2");

                    b.Property<string>("NoName3");

                    b.Property<string>("NoName4");

                    b.Property<string>("NoName5");

                    b.Property<string>("NoName6");

                    b.Property<string>("Phone");

                    b.Property<string>("Price");

                    b.Property<string>("TimeOfAppointment");

                    b.Property<string>("TimeOfPickup");

                    b.Property<string>("ToAddress");

                    b.HasKey("ID");

                    b.ToTable("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}