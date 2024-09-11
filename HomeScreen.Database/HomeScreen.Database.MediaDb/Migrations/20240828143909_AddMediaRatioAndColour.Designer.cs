﻿// <auto-generated />
using System;
using HomeScreen.Database.MediaDb.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HomeScreen.Database.MediaDb.Migrations
{
    [DbContext(typeof(MediaDbContext))]
    [Migration("20240828143909_AddMediaRatioAndColour")]
    partial class AddMediaRatioAndColour
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HomeScreen.Database.MediaDb.Entities.MediaEntry", b =>
                {
                    b.Property<Guid>("MediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("BaseColourB")
                        .HasColumnType("real");

                    b.Property<float>("BaseColourG")
                        .HasColumnType("real");

                    b.Property<float>("BaseColourR")
                        .HasColumnType("real");

                    b.Property<TimeSpan>("CapturedOffset")
                        .HasColumnType("time");

                    b.Property<DateTimeOffset>("CapturedUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<double>("ImageRatio")
                        .HasColumnType("float");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<int>("LatitudeDirection")
                        .HasColumnType("int");

                    b.Property<string>("LocationLabel")
                        .IsRequired()
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<int>("LongitudeDirection")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(1048576)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginalFile")
                        .IsRequired()
                        .HasMaxLength(260)
                        .HasColumnType("nvarchar(260)");

                    b.Property<string>("OriginalHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("MediaId");

                    b.ToTable("MediaEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
