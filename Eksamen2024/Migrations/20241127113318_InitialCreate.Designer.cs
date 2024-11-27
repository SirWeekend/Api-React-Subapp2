﻿// <auto-generated />
using System;
using Eksamen2024.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Eksamen2024.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241127113318_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("Eksamen2024.Models.Admin", b =>
                {
                    b.Property<int>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AdminId");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("Eksamen2024.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AdminId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PinpointId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CommentId");

                    b.HasIndex("AdminId");

                    b.HasIndex("PinpointId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Eksamen2024.Models.Pinpoint", b =>
                {
                    b.Property<int>("PinpointId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AdminId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PinpointId");

                    b.HasIndex("AdminId");

                    b.HasIndex("UserId");

                    b.ToTable("Pinpoints");
                });

            modelBuilder.Entity("Eksamen2024.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Eksamen2024.Models.Comment", b =>
                {
                    b.HasOne("Eksamen2024.Models.Admin", null)
                        .WithMany("Comments")
                        .HasForeignKey("AdminId");

                    b.HasOne("Eksamen2024.Models.Pinpoint", "Pinpoint")
                        .WithMany("Comments")
                        .HasForeignKey("PinpointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eksamen2024.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Pinpoint");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Eksamen2024.Models.Pinpoint", b =>
                {
                    b.HasOne("Eksamen2024.Models.Admin", null)
                        .WithMany("Pinpoint")
                        .HasForeignKey("AdminId");

                    b.HasOne("Eksamen2024.Models.User", "User")
                        .WithMany("Pinpoint")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Eksamen2024.Models.Admin", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Pinpoint");
                });

            modelBuilder.Entity("Eksamen2024.Models.Pinpoint", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Eksamen2024.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Pinpoint");
                });
#pragma warning restore 612, 618
        }
    }
}
