﻿// <auto-generated />
using System;
using CVBuilder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CVBuilder.Data.Migrations
{
    [DbContext(typeof(CVBuilderDbContext))]
    [Migration("20250518180251_dbuse")]
    partial class dbuse
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("CVBuilder.Core.Models.FileCV", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "email");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "firstName");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "lastName");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "Phone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "role");

                    b.PrimitiveCollection<string>("Skills")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "skills");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasAnnotation("Relational:JsonPropertyName", "summary");

                    b.Property<string>("Template")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FileCVs");
                });

            modelBuilder.Entity("CVBuilder.Core.Models.Template", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("InUse")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TemplateUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("CVBuilder.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CVBuilder.Core.Models.FileCV", b =>
                {
                    b.HasOne("CVBuilder.Core.Models.User", "User")
                        .WithMany("CVFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("CVBuilder.Core.Models.Education", "Educations", b1 =>
                        {
                            b1.Property<int>("FileCVId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Degree")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "degree");

                            b1.Property<string>("Institution")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "institution");

                            b1.HasKey("FileCVId", "Id");

                            b1.ToTable("Education");

                            b1.HasAnnotation("Relational:JsonPropertyName", "educations");

                            b1.WithOwner()
                                .HasForeignKey("FileCVId");
                        });

                    b.OwnsMany("CVBuilder.Core.Models.Language", "Languages", b1 =>
                        {
                            b1.Property<int>("FileCVId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("LanguageName")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "languageName");

                            b1.Property<string>("Level")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "level");

                            b1.HasKey("FileCVId", "Id");

                            b1.ToTable("Language");

                            b1.HasAnnotation("Relational:JsonPropertyName", "languages");

                            b1.WithOwner()
                                .HasForeignKey("FileCVId");
                        });

                    b.OwnsMany("CVBuilder.Core.Models.WorkExperience", "WorkExperiences", b1 =>
                        {
                            b1.Property<int>("FileCVId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Company")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "company");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "description");

                            b1.Property<string>("EndDate")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "endDate");

                            b1.Property<string>("Position")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "position");

                            b1.Property<string>("StartDate")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasAnnotation("Relational:JsonPropertyName", "startDate");

                            b1.HasKey("FileCVId", "Id");

                            b1.ToTable("WorkExperience");

                            b1.HasAnnotation("Relational:JsonPropertyName", "workExperiences");

                            b1.WithOwner()
                                .HasForeignKey("FileCVId");
                        });

                    b.Navigation("Educations");

                    b.Navigation("Languages");

                    b.Navigation("User");

                    b.Navigation("WorkExperiences");
                });

            modelBuilder.Entity("CVBuilder.Core.Models.User", b =>
                {
                    b.Navigation("CVFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
