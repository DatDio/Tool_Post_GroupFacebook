﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tool_Facebook.Data;

namespace Tool_Facebook.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240315105238_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.32");

            modelBuilder.Entity("Tool_Facebook.Data.tblPage", b =>
                {
                    b.Property<string>("C_IDPage")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Follower")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_NamePage")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_StatusPage")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_UID")
                        .HasColumnType("TEXT");

                    b.Property<string>("tblViaC_UID")
                        .HasColumnType("TEXT");

                    b.HasKey("C_IDPage");

                    b.HasIndex("tblViaC_UID");

                    b.ToTable("tblPages");
                });

            modelBuilder.Entity("Tool_Facebook.Data.tblVia", b =>
                {
                    b.Property<string>("C_UID")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_2FA")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Cookie")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Folder")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_PassEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Proxy")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Status")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_Token")
                        .HasColumnType("TEXT");

                    b.Property<string>("C_UserAgent")
                        .HasColumnType("TEXT");

                    b.HasKey("C_UID");

                    b.ToTable("tblViaPage");
                });

            modelBuilder.Entity("Tool_Facebook.Data.tblPage", b =>
                {
                    b.HasOne("Tool_Facebook.Data.tblVia", "tblVia")
                        .WithMany("Pages")
                        .HasForeignKey("tblViaC_UID");
                });
#pragma warning restore 612, 618
        }
    }
}
