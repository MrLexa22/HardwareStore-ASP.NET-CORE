﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KursovoiProject_ElShop
{
    public partial class ElShopContext : DbContext
    {
        public ElShopContext()
        {
        }

        public ElShopContext(DbContextOptions<ElShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddsSite> AddsSites { get; set; } = null!;
        public virtual DbSet<Bank> Banks { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Filial> Filials { get; set; } = null!;
        public virtual DbSet<Good> Goods { get; set; } = null!;
        public virtual DbSet<GoodsFilial> GoodsFilials { get; set; } = null!;
        public virtual DbSet<ViewGoodsWithManufacture> ViewGoodsWithManufactures { get; set; } = null!;
        public virtual DbSet<Korzina> Korzinas { get; set; } = null!;
        public virtual DbSet<MainCategory> MainCategories { get; set; } = null!;
        public virtual DbSet<Manufacture> Manufactures { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UsersRole> UsersRoles { get; set; } = null!;
        public virtual DbSet<ViewMainCategori> ViewMainCategoris { get; set; } = null!;
        public virtual DbSet<WorkersBankCadri> WorkersBankCadris { get; set; } = null!;
        public virtual DbSet<WorkersFilial> WorkersFilials { get; set; } = null!;
        public virtual DbSet<WorkersPost> WorkersPosts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=89.108.64.223;user=remoteDB;password=ietie7chohmo;database=ElShop", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            modelBuilder.Entity<AddsSite>(entity =>
            {
                entity.HasKey(e => e.IdAdds)
                    .HasName("PRIMARY");

                entity.ToTable("AddsSite");

                entity.Property(e => e.IdAdds).HasColumnName("ID_Adds");

                entity.Property(e => e.FtppathImage)
                    .HasColumnType("text")
                    .HasColumnName("FTPPathImage");

                entity.Property(e => e.Href)
                    .HasColumnType("text")
                    .HasColumnName("HREF");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.HasKey(e => e.IdBank)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdBank).HasColumnName("ID_Bank");

                entity.Property(e => e.BikBank).HasMaxLength(10);

                entity.Property(e => e.InnBank).HasMaxLength(11);

                entity.Property(e => e.KorrSchetBank).HasMaxLength(20);

                entity.Property(e => e.KppBank).HasMaxLength(10);

                entity.Property(e => e.NameBank).HasMaxLength(250);
            });

            modelBuilder.Entity<ViewGoodsWithManufacture>(entity =>
            {
                entity.HasKey(e => e.IdGood)
                    .HasName("PRIMARY");

                entity.ToView("View_GoodsWithManufacture");

                entity.Property(e => e.CategoriId).HasColumnName("Categori_ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.FtppathImage)
                    .HasMaxLength(150)
                    .HasColumnName("FTPPathImage");

                entity.Property(e => e.IdGood).HasColumnName("ID_Good");

                entity.Property(e => e.IdManufacture).HasColumnName("ID_Manufacture");

                entity.Property(e => e.ManufactureId).HasColumnName("Manufacture_ID");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.NameManufacture).HasMaxLength(100);

                entity.Property(e => e.NameWithManufacture).HasMaxLength(251);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.IdCategori)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdCategori).HasColumnName("ID_Categori");

                entity.Property(e => e.NameCategori).HasMaxLength(100);
            });

            modelBuilder.Entity<Filial>(entity =>
            {
                entity.HasKey(e => e.IdFilial)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdFilial).HasColumnName("ID_Filial");

                entity.Property(e => e.AddressFilial).HasColumnType("text");

                entity.Property(e => e.NameFilial).HasMaxLength(100);
            });

            modelBuilder.Entity<Good>(entity =>
            {
                entity.HasKey(e => e.IdGood)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CategoriId, "CategoriID_ForeignKey");

                entity.HasIndex(e => e.ManufactureId, "ManufactureID_ForeignKey");

                entity.Property(e => e.IdGood).HasColumnName("ID_Good");

                entity.Property(e => e.CategoriId).HasColumnName("Categori_ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.FtppathImage)
                    .HasMaxLength(150)
                    .HasColumnName("FTPPathImage");

                entity.Property(e => e.ManufactureId).HasColumnName("Manufacture_ID");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.HasOne(d => d.Categori)
                    .WithMany(p => p.Goods)
                    .HasForeignKey(d => d.CategoriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Goods_ibfk_1");

                entity.HasOne(d => d.Manufacture)
                    .WithMany(p => p.Goods)
                    .HasForeignKey(d => d.ManufactureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Goods_ibfk_2");
            });

            modelBuilder.Entity<GoodsFilial>(entity =>
            {
                entity.HasKey(e => e.IdGoodsFilial)
                    .HasName("PRIMARY");

                entity.ToTable("Goods_Filial");

                entity.HasIndex(e => e.FilialId, "FilialID_ForeignKey");

                entity.HasIndex(e => e.GoodsId, "GoodsID_ForeignKey");

                entity.Property(e => e.IdGoodsFilial).HasColumnName("ID_GoodsFilial");

                entity.Property(e => e.FilialId).HasColumnName("Filial_ID");

                entity.Property(e => e.GoodsId).HasColumnName("Goods_ID");

                entity.HasOne(d => d.Filial)
                    .WithMany(p => p.GoodsFilials)
                    .HasForeignKey(d => d.FilialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Goods_Filial_ibfk_2");

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.GoodsFilials)
                    .HasForeignKey(d => d.GoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Goods_Filial_ibfk_1");
            });

            modelBuilder.Entity<Korzina>(entity =>
            {
                entity.HasKey(e => e.IdKorzinaGood)
                    .HasName("PRIMARY");

                entity.ToTable("Korzina");

                entity.HasIndex(e => e.GoodsId, "GoodsID_ForeignKey");

                entity.HasIndex(e => e.UserId, "UserID_ForeignKey");

                entity.Property(e => e.IdKorzinaGood).HasColumnName("ID_KorzinaGood");

                entity.Property(e => e.GoodsId).HasColumnName("Goods_ID");

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.Korzinas)
                    .HasForeignKey(d => d.GoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Korzina_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Korzinas)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Korzina_ibfk_2");
            });

            modelBuilder.Entity<MainCategory>(entity =>
            {
                entity.HasKey(e => e.IdMainCategori)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CategoriId, "CategoriID_ForeignKey");

                entity.Property(e => e.IdMainCategori).HasColumnName("ID_MainCategori");

                entity.Property(e => e.CategoriId).HasColumnName("CategoriID");

                entity.HasOne(d => d.Categori)
                    .WithMany(p => p.MainCategories)
                    .HasForeignKey(d => d.CategoriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MainCategories_ibfk_1");
            });

            modelBuilder.Entity<Manufacture>(entity =>
            {
                entity.HasKey(e => e.IdManufacture)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdManufacture).HasColumnName("ID_Manufacture");

                entity.Property(e => e.NameManufacture).HasMaxLength(100);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.FilialId, "FilialID_ForeignKey");

                entity.HasIndex(e => e.GoodsId, "GoodsID_Index");

                entity.HasIndex(e => e.UserId, "User_ID_ForeignKey");

                entity.Property(e => e.Address).HasColumnType("text");

                entity.Property(e => e.DateDostavki).HasColumnType("datetime");

                entity.Property(e => e.DateOrder).HasColumnType("datetime");

                entity.Property(e => e.FilialId).HasColumnName("Filial_ID");

                entity.Property(e => e.GoodsId).HasColumnName("Goods_ID");

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.Filial)
                    .WithMany()
                    .HasForeignKey(d => d.FilialId)
                    .HasConstraintName("Orders_ibfk_3");

                entity.HasOne(d => d.Goods)
                    .WithMany()
                    .HasForeignKey(d => d.GoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Orders_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Orders_ibfk_1");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.IdPost)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdPost).HasColumnName("ID_Post");

                entity.Property(e => e.NamePost).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.IdRole)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdRole).HasColumnName("ID_Role");

                entity.Property(e => e.NameRole).HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdUser).HasColumnName("ID_User");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Login).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasKey(e => e.IdUserRole)
                    .HasName("PRIMARY");

                entity.ToTable("Users_Role");

                entity.HasIndex(e => e.RoleId, "Role_ID_ForeignKey");

                entity.HasIndex(e => e.UserId, "User_ID_ForeignKey");

                entity.Property(e => e.IdUserRole).HasColumnName("ID_UserRole");

                entity.Property(e => e.RoleId).HasColumnName("Role_ID");

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Users_Role_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Users_Role_ibfk_1");
            });

            modelBuilder.Entity<ViewMainCategori>(entity =>
            {
                entity.HasKey(e => e.IdCategori)
                    .HasName("PRIMARY");

                entity.ToView("View_MainCategori");

                entity.Property(e => e.IdCategori).HasColumnName("ID_Categori");

                entity.Property(e => e.NameCategori).HasMaxLength(100);
            });

            modelBuilder.Entity<WorkersBankCadri>(entity =>
            {
                entity.HasKey(e => e.IdBankWorker)
                    .HasName("PRIMARY");

                entity.ToTable("Workers_BankCadri");

                entity.HasIndex(e => e.BankId, "BankID_ForeignKey");

                entity.HasIndex(e => e.UserId, "UserID_ForeignKey");

                entity.Property(e => e.IdBankWorker).HasColumnName("ID_BankWorker");

                entity.Property(e => e.BankId).HasColumnName("BankID");

                entity.Property(e => e.Innworker)
                    .HasMaxLength(13)
                    .HasColumnName("INNWorker");

                entity.Property(e => e.NomerPasporta).HasMaxLength(10);

                entity.Property(e => e.PrikazOprieme)
                    .HasMaxLength(150)
                    .HasColumnName("PrikazOPrieme");

                entity.Property(e => e.SchetWorker).HasMaxLength(21);

                entity.Property(e => e.SeriaPasporta).HasMaxLength(5);

                entity.Property(e => e.Snils)
                    .HasMaxLength(20)
                    .HasColumnName("SNILS");

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.WorkersBankCadris)
                    .HasForeignKey(d => d.BankId)
                    .HasConstraintName("Workers_BankCadri_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WorkersBankCadris)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Workers_BankCadri_ibfk_2");
            });

            modelBuilder.Entity<WorkersFilial>(entity =>
            {
                entity.HasKey(e => e.IdWorkerFilial)
                    .HasName("PRIMARY");

                entity.ToTable("Workers_Filial");

                entity.HasIndex(e => e.FilialId, "FilialID_ForeignKey");

                entity.HasIndex(e => e.UserId, "User_ID_ForeignKey");

                entity.Property(e => e.IdWorkerFilial).HasColumnName("ID_WorkerFilial");

                entity.Property(e => e.FilialId).HasColumnName("Filial_ID");

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.Filial)
                    .WithMany(p => p.WorkersFilials)
                    .HasForeignKey(d => d.FilialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Workers_Filial_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WorkersFilials)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Workers_Filial_ibfk_2");
            });

            modelBuilder.Entity<WorkersPost>(entity =>
            {
                entity.HasKey(e => e.IdWorker)
                    .HasName("PRIMARY");

                entity.ToTable("Workers_Post");

                entity.HasIndex(e => e.PostId, "Post_ID_ForeignKey");

                entity.HasIndex(e => e.UserId, "User_ID_ForeignKey");

                entity.Property(e => e.IdWorker).HasColumnName("ID_Worker");

                entity.Property(e => e.PostId).HasColumnName("Post_ID");

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.WorkersPosts)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Workers_Post_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WorkersPosts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Workers_Post_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
