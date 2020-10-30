﻿// <auto-generated />
using System;
using MMBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MMBot.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9");

            modelBuilder.Entity("MMBot.Data.Entities.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("AnswerTextChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("TextChannelId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Channel");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Clan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DiscordRole")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("SortOrder")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SortOrder")
                        .IsUnique();

                    b.HasIndex("Tag")
                        .IsUnique();

                    b.ToTable("Clan");
                });

            modelBuilder.Entity("MMBot.Data.Entities.GuildSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClanSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberMovementQty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Prefix")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("WaitForReaction")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("MMBot.Data.Entities.MMTimer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRecurring")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan?>("RingSpan")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Timer");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AHigh")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AutoSignUpForFightNight")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ClanId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discord")
                        .HasColumnType("TEXT");

                    b.Property<int>("DiscordStatus")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Donations")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IgnoreOnMoveUp")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Join")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MemberGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PlayerTag")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SHigh")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClanId");

                    b.HasIndex("MemberGroupId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Member");
                });

            modelBuilder.Entity("MMBot.Data.Entities.MemberGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("MemberGroup");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Restart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Guild")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Restart");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Vacation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("MemberId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Vacation");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Member", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Clan", "Clan")
                        .WithMany("Member")
                        .HasForeignKey("ClanId");

                    b.HasOne("MMBot.Data.Entities.MemberGroup", "MemberGroup")
                        .WithMany("Members")
                        .HasForeignKey("MemberGroupId");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Vacation", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Member", "Member")
                        .WithMany("Vacation")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
