﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrunkRings.DAL;

#nullable disable

namespace TrunkRings.DAL.Migrations
{
    [DbContext(typeof(SecretaryContext))]
    [Migration("20220813144308_MessageDateToUtc")]
    partial class MessageDateToUtc
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TrunkRings.DAL.Models.AdminDataSet", b =>
                {
                    b.Property<Guid>("AdminDataSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("AddTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("AddedUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("AddedUserName")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("DeleteTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("DeletedUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("DeletedUserName")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("AdminDataSetId");

                    b.ToTable("AdminDataSets", "public");
                });

            modelBuilder.Entity("TrunkRings.DAL.Models.BookkeeperDataSet", b =>
                {
                    b.Property<Guid>("BookkeeperDataSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("UserFirstName")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserLastName")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("BookkeeperDataSetId");

                    b.ToTable("BookkeeperDataSets", "public");
                });

            modelBuilder.Entity("TrunkRings.DAL.Models.MessageDataSet", b =>
                {
                    b.Property<Guid>("MessageDataSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("ChatName")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<long>("MessageId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserFirstName")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserLastName")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("MessageDataSetId");

                    b.ToTable("MessageDataSets", "public");
                });

            modelBuilder.Entity("TrunkRings.DAL.Models.OnetimeChatDataSet", b =>
                {
                    b.Property<Guid>("OnetimeChatDataSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("ChatName")
                        .HasColumnType("text");

                    b.HasKey("OnetimeChatDataSetId");

                    b.ToTable("OnetimeChatDataSets", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
