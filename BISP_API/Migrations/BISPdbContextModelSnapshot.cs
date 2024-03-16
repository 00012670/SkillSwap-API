﻿// <auto-generated />
using System;
using BISP_API.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BISP_API.Migrations
{
    [DbContext(typeof(BISPdbContext))]
    partial class BISPdbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BISP_API.Models.Image", b =>
                {
                    b.Property<int>("ImgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImgId"), 1L, 1);

                    b.Property<byte[]>("Img")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ImgId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Images");
                });

            modelBuilder.Entity("BISP_API.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReviewId"), 1L, 1);

                    b.Property<int>("FromUserId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<int>("SkillId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ToUserId")
                        .HasColumnType("int");

                    b.HasKey("ReviewId");

                    b.HasIndex("FromUserId");

                    b.HasIndex("RequestId");

                    b.HasIndex("SkillId");

                    b.HasIndex("ToUserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("BISP_API.Models.Skill", b =>
                {
                    b.Property<int>("SkillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SkillId"), 1L, 1);

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prerequisity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SkillId");

                    b.HasIndex("UserId");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("BISP_API.Models.SwapRequest", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestId"), 1L, 1);

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InitiatorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ReceiverId")
                        .HasColumnType("int");

                    b.Property<int>("SkillOfferedId")
                        .HasColumnType("int");

                    b.Property<int>("SkillRequestedId")
                        .HasColumnType("int");

                    b.Property<int>("StatusRequest")
                        .HasColumnType("int");

                    b.HasKey("RequestId");

                    b.HasIndex("InitiatorId");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SkillOfferedId");

                    b.HasIndex("SkillRequestedId");

                    b.ToTable("SwapRequests");
                });

            modelBuilder.Entity("BISP_API.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HasImage")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SkillInterested")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("BISP_API.Models.Image", b =>
                {
                    b.HasOne("BISP_API.Models.User", "User")
                        .WithOne("ProfileImage")
                        .HasForeignKey("BISP_API.Models.Image", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BISP_API.Models.Review", b =>
                {
                    b.HasOne("BISP_API.Models.User", "FromUser")
                        .WithMany("ReviewsSent")
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BISP_API.Models.SwapRequest", "Request")
                        .WithMany("Reviews")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BISP_API.Models.Skill", "Skill")
                        .WithMany("Reviews")
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BISP_API.Models.User", "ToUser")
                        .WithMany("ReviewsReceived")
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("FromUser");

                    b.Navigation("Request");

                    b.Navigation("Skill");

                    b.Navigation("ToUser");
                });

            modelBuilder.Entity("BISP_API.Models.Skill", b =>
                {
                    b.HasOne("BISP_API.Models.User", "User")
                        .WithMany("Skills")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BISP_API.Models.SwapRequest", b =>
                {
                    b.HasOne("BISP_API.Models.User", "Initiator")
                        .WithMany("SwapRequestsInitiated")
                        .HasForeignKey("InitiatorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BISP_API.Models.User", "Receiver")
                        .WithMany("SwapRequestsReceived")
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BISP_API.Models.Skill", "SkillOffered")
                        .WithMany("SwapRequestsOffered")
                        .HasForeignKey("SkillOfferedId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BISP_API.Models.Skill", "SkillRequested")
                        .WithMany("SwapRequestsExchanged")
                        .HasForeignKey("SkillRequestedId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Initiator");

                    b.Navigation("Receiver");

                    b.Navigation("SkillOffered");

                    b.Navigation("SkillRequested");
                });

            modelBuilder.Entity("BISP_API.Models.Skill", b =>
                {
                    b.Navigation("Reviews");

                    b.Navigation("SwapRequestsExchanged");

                    b.Navigation("SwapRequestsOffered");
                });

            modelBuilder.Entity("BISP_API.Models.SwapRequest", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("BISP_API.Models.User", b =>
                {
                    b.Navigation("ProfileImage");

                    b.Navigation("ReviewsReceived");

                    b.Navigation("ReviewsSent");

                    b.Navigation("Skills");

                    b.Navigation("SwapRequestsInitiated");

                    b.Navigation("SwapRequestsReceived");
                });
#pragma warning restore 612, 618
        }
    }
}
