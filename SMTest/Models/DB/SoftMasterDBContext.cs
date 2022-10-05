using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SMTest.Models.DB
{
    public partial class SoftMasterDBContext : DbContext
    {
        public SoftMasterDBContext()
        {
        }

        public SoftMasterDBContext(DbContextOptions<SoftMasterDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<BusyPicket> BusyPickets { get; set; }
        public virtual DbSet<BusyPicketsHistory> BusyPicketsHistories { get; set; }
        public virtual DbSet<FreePicket> FreePickets { get; set; }
        public virtual DbSet<FreePicketsHistory> FreePicketsHistories { get; set; }
        public virtual DbSet<WareHouse> WareHouses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-UL795I5\\SQLEXPRESS;Database=SoftMasterDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Площадка '+rand((100000)))");

                entity.HasOne(d => d.WareHouse)
                    .WithMany(p => p.Areas)
                    .HasForeignKey(d => d.WareHouseId)
                    .HasConstraintName("FK_Areas_WareHouses");
            });

            modelBuilder.Entity<BusyPicket>(entity =>
            {
                entity.HasKey(e => e.PicketId)
                    .HasName("PK__BusyPick__4F2F1E4680FE6BBE");

                entity.Property(e => e.DateStart)
                    .HasColumnName("Date_start")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.BusyPickets)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK_BusyPickets_Areas");
            });

            modelBuilder.Entity<BusyPicketsHistory>(entity =>
            {
                entity.HasKey(e => e.PicketId)
                    .HasName("PK__BusyPick__4F2F1E464396A582");

                entity.ToTable("BusyPicketsHistory");

                entity.Property(e => e.DateEnd)
                    .HasColumnName("Date_end")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateStart).HasColumnName("Date_start");
            });

            modelBuilder.Entity<FreePicket>(entity =>
            {
                entity.HasKey(e => e.PicketId)
                    .HasName("PK__FreePick__4F2F1E4662BB69ED");

                entity.Property(e => e.DateStart)
                    .HasColumnName("Date_start")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.WareHouse)
                    .WithMany(p => p.FreePickets)
                    .HasForeignKey(d => d.WareHouseId)
                    .HasConstraintName("FK_FreePickets_WareHouses");
            });

            modelBuilder.Entity<FreePicketsHistory>(entity =>
            {
                entity.HasKey(e => e.PicketId)
                    .HasName("PK__FreePick__4F2F1E46C9C4C3C5");

                entity.ToTable("FreePicketsHistory");

                entity.Property(e => e.DateEnd)
                    .HasColumnName("Date_end")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateStart).HasColumnName("Date_start");
            });

            modelBuilder.Entity<WareHouse>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Склад '+rand((100000)))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
