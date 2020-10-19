using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace webServer.Models
{
    public partial class imdbContext : DbContext
    {
        public imdbContext()
        {
        }

        public imdbContext(DbContextOptions<imdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accountinfo> Accountinfo { get; set; }
        public virtual DbSet<Groupinfo> Groupinfo { get; set; }
        public virtual DbSet<Msginfo> Msginfo { get; set; }
        public virtual DbSet<Userinfo> Userinfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=118.31.127.222;userid=root;pwd=di3k5mnv83mca4;port=3307;database=imdb;sslmode=none", x => x.ServerVersion("10.5.6-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accountinfo>(entity =>
            {
                entity.ToTable("accountinfo");

                entity.HasIndex(e => e.AppSecret)
                    .HasName("accid")
                    .IsUnique();

                entity.HasIndex(e => e.Appid)
                    .HasName("appid");

                entity.HasIndex(e => new { e.AppSecret, e.Appid })
                    .HasName("accidappid")
                    .IsUnique()
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AppSecret)
                    .HasColumnName("appSecret")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Appid)
                    .HasColumnName("appid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Createtime)
                    .HasColumnName("createtime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Groupinfo>(entity =>
            {
                entity.ToTable("groupinfo");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Accid)
                    .HasColumnName("accid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Appid)
                    .HasColumnName("appid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Createtime)
                    .HasColumnName("createtime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Groupid)
                    .HasColumnName("groupid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Msginfo>(entity =>
            {
                entity.ToTable("msginfo");

                entity.HasIndex(e => e.Appid)
                    .HasName("appid");

                entity.HasIndex(e => e.From)
                    .HasName("from");

                entity.HasIndex(e => e.Ope)
                    .HasName("ope");

                entity.HasIndex(e => e.To)
                    .HasName("to");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Appid)
                    .HasColumnName("appid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Body)
                    .HasColumnName("body")
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Createtime)
                    .HasColumnName("createtime")
                    .HasColumnType("datetime");

                entity.Property(e => e.From)
                    .HasColumnName("from")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Ope)
                    .HasColumnName("ope")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Readstatus)
                    .HasColumnName("readstatus")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Readtime)
                    .HasColumnName("readtime")
                    .HasColumnType("datetime");

                entity.Property(e => e.To)
                    .HasColumnName("to")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Userinfo>(entity =>
            {
                entity.ToTable("userinfo");

                entity.HasIndex(e => e.Accid)
                    .HasName("accid")
                    .IsUnique();

                entity.HasIndex(e => e.Appid)
                    .HasName("appid");

                entity.HasIndex(e => new { e.Accid, e.Appid })
                    .HasName("accidappid")
                    .IsUnique()
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Accid)
                    .HasColumnName("accid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Appid)
                    .HasColumnName("appid")
                    .HasColumnType("varchar(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Createtime)
                    .HasColumnName("createtime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
