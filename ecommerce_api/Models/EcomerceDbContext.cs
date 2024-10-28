using System;
using System.Collections.Generic;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Models;

public partial class EcomerceDbContext : IdentityDbContext<ApplicationUser>
{
    public EcomerceDbContext()
    {
    }

    public EcomerceDbContext(DbContextOptions<EcomerceDbContext> options)
        : base(options)
    {
    }

    

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewsImage> ReviewsImages { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<ShopCategory> ShopCategories { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<VoucherCategory> VoucherCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });
        modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
        modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.OrderStatusId, "IX_Orders_OrderStatusId");

            

            entity.HasIndex(e => e.PaymentId, "IX_Orders_PaymentId");

            entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

            entity.HasIndex(e => e.VoucherId, "IX_Orders_VoucherId");

            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.OrderOrderStatuses)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders).HasForeignKey(d => d.PaymentId);

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders).HasForeignKey(d => d.VoucherId);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderDetails_ProductId");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.BrandId, "IX_Products_BrandId");

            entity.HasIndex(e => e.ProductCategoryId, "IX_Products_ProductCategoryId");

            entity.HasIndex(e => e.ShopId, "IX_Products_ShopId");

            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaSp)
                .HasMaxLength(20)
                .HasColumnName("MaSP");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasForeignKey(d => d.BrandId);

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products).HasForeignKey(d => d.ProductCategoryId);

            entity.HasOne(d => d.Shop).WithMany(p => p.Products).HasForeignKey(d => d.ShopId);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImage");

            entity.HasIndex(e => e.ProductId, "IX_ProductImage_ProductId");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewsId);

            entity.HasIndex(e => e.CustomerId, "IX_Reviews_CustomerId");

            entity.HasIndex(e => e.ProductId, "IX_Reviews_ProductId");


            entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<ReviewsImage>(entity =>
        {
            entity.ToTable("ReviewsImage");

            entity.HasIndex(e => e.ReviewsId, "IX_ReviewsImage_ReviewsId");

            entity.HasOne(d => d.Reviews).WithMany(p => p.ReviewsImages).HasForeignKey(d => d.ReviewsId);
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasIndex(e => e.ShopCategoryId, "IX_Shops_ShopCategoryId");

            entity.HasOne(d => d.ShopCategory).WithMany(p => p.Shops).HasForeignKey(d => d.ShopCategoryId);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasIndex(e => e.VoucherCategoryId, "IX_Vouchers_VoucherCategoryId");

            entity.Property(e => e.DonToiThieu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiamToiDa).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.VoucherCategory).WithMany(p => p.Vouchers).HasForeignKey(d => d.VoucherCategoryId);
        });

        modelBuilder.Entity<VoucherCategory>(entity =>
        {
            entity.ToTable("VoucherCategory");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
