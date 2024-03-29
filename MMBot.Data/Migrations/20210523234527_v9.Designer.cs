﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MMBot.Data.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20210523234527_v9")]
    partial class v9
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MMBot.Data.Entities.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("AnswerTextChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("TextChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("Channel");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Clan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("DiscordRole")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("SortOrder")
                        .HasColumnType("integer");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("Tag", "Name", "GuildId", "SortOrder")
                        .IsUnique();

                    b.ToTable("Clan");
                });

            modelBuilder.Entity("MMBot.Data.Entities.GuildSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClanSize")
                        .HasColumnType("integer");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("GuildName")
                        .HasColumnType("text");

                    b.Property<int>("MemberMovementQty")
                        .HasColumnType("integer");

                    b.Property<string>("Prefix")
                        .HasColumnType("text");

                    b.Property<TimeSpan>("WaitForReaction")
                        .HasColumnType("interval");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("MMBot.Data.Entities.MMTimer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal?>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRecurring")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan?>("RingSpan")
                        .HasColumnType("interval");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("Timer");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AHigh")
                        .HasColumnType("integer");

                    b.Property<bool>("AutoSignUpForFightNight")
                        .HasColumnType("boolean");

                    b.Property<int?>("ClanId")
                        .HasColumnType("integer");

                    b.Property<int?>("Current")
                        .HasColumnType("integer");

                    b.Property<string>("Discord")
                        .HasColumnType("text");

                    b.Property<int>("DiscordStatus")
                        .HasColumnType("integer");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("IgnoreOnMoveUp")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("Join")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double?>("LocalTimeOffSet")
                        .HasColumnType("double precision");

                    b.Property<int?>("MemberGroupId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerTag")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("ClanId");

                    b.HasIndex("MemberGroupId");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("MMBot.Data.Entities.MemberGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("MemberGroup");
                });

            modelBuilder.Entity("MMBot.Data.Entities.RaidBoss", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte>("BossType")
                        .HasColumnType("smallint");

                    b.Property<int>("ClanId")
                        .HasColumnType("integer");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<byte>("ModifierOne")
                        .HasColumnType("smallint");

                    b.Property<byte>("ModifierThree")
                        .HasColumnType("smallint");

                    b.Property<byte>("ModifierTwo")
                        .HasColumnType("smallint");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("ClanId");

                    b.ToTable("RaidBoss");
                });

            modelBuilder.Entity("MMBot.Data.Entities.RaidParticipation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AttackQty")
                        .HasColumnType("integer");

                    b.Property<decimal>("DamageDone")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("HeartQty")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<int>("RaidBossId")
                        .HasColumnType("integer");

                    b.Property<int>("RaidParticipationId")
                        .HasColumnType("integer");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("RaidParticipationId");

                    b.ToTable("RaidParticipation");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Restart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Channel")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("DBImport")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Guild")
                        .HasColumnType("numeric(20,0)");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("Restart");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Season", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte>("Era")
                        .HasColumnType("smallint");

                    b.Property<int>("No")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp without time zone");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("Season");
                });

            modelBuilder.Entity("MMBot.Data.Entities.SeasonResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("Donations")
                        .HasColumnType("integer");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<int>("No")
                        .HasColumnType("integer");

                    b.Property<int?>("SHigh")
                        .HasColumnType("integer");

                    b.Property<int>("SeasonId")
                        .HasColumnType("integer");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("SeasonId");

                    b.ToTable("SeasonResult");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Strike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.Property<DateTime?>("StrikeDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Strike");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Vacation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

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

                    b.Navigation("Clan");

                    b.Navigation("MemberGroup");
                });

            modelBuilder.Entity("MMBot.Data.Entities.RaidBoss", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Clan", "Clan")
                        .WithMany("RaidBoss")
                        .HasForeignKey("ClanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clan");
                });

            modelBuilder.Entity("MMBot.Data.Entities.RaidParticipation", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Member", "Member")
                        .WithMany("RaidParticipation")
                        .HasForeignKey("RaidParticipationId")
                        .IsRequired();

                    b.HasOne("MMBot.Data.Entities.RaidBoss", "RaidBoss")
                        .WithMany("RaidParticipation")
                        .HasForeignKey("RaidParticipationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("RaidBoss");
                });

            modelBuilder.Entity("MMBot.Data.Entities.SeasonResult", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Member", "Member")
                        .WithMany("SeasonResult")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("MMBot.Data.Entities.Season", "Season")
                        .WithMany("SeasonResult")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Strike", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Member", "Member")
                        .WithMany("Strike")
                        .HasForeignKey("MemberId")
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Vacation", b =>
                {
                    b.HasOne("MMBot.Data.Entities.Member", "Member")
                        .WithMany("Vacation")
                        .HasForeignKey("MemberId")
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Clan", b =>
                {
                    b.Navigation("Member");

                    b.Navigation("RaidBoss");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Member", b =>
                {
                    b.Navigation("RaidParticipation");

                    b.Navigation("SeasonResult");

                    b.Navigation("Strike");

                    b.Navigation("Vacation");
                });

            modelBuilder.Entity("MMBot.Data.Entities.MemberGroup", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("MMBot.Data.Entities.RaidBoss", b =>
                {
                    b.Navigation("RaidParticipation");
                });

            modelBuilder.Entity("MMBot.Data.Entities.Season", b =>
                {
                    b.Navigation("SeasonResult");
                });
#pragma warning restore 612, 618
        }
    }
}
