using DataAccessObject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class MFashionStoreDBContext : DbContext
    {
        public MFashionStoreDBContext() { }

        public MFashionStoreDBContext(DbContextOptions<MFashionStoreDBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsetting.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<PendingAccount> PendingAccounts { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogImage> BlogsImages { get; set; }
        public DbSet<BlogTag> BlogsTags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewsImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.ShopName)
                .IsUnique()
                .HasFilter("[ShopName] IS NOT NULL");

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Slug)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            // Configure 1-N relationship between Role and Account
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId);

            // Configure 1-N relationship between Account and Blog
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Account)
                .WithMany(a => a.Blogs)
                .HasForeignKey(b => b.AccountId);

            // Configure 1-N relationship between Account and Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Account)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AccountId);

            // Configure 1-N relationship between Blog and Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Blog and BlogImage
            modelBuilder.Entity<BlogImage>()
                .HasOne(bi => bi.Blog)
                .WithMany(b => b.BlogImages)
                .HasForeignKey(bi => bi.BlogId);

            // Configure 1-N relationship between BlogCategory and Blog
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.BlogCategory)
                .WithMany(bc => bc.Blogs)
                .HasForeignKey(b => b.CategoryId);

            // Configure N-N relationship between Blog and BlogTag
            modelBuilder.Entity<BlogTag>()
                .HasMany(bt => bt.Blogs)
                .WithMany(b => b.BlogTags)
                .UsingEntity(j => j.ToTable("BlogTags"));

            // Configure 1-N relationship between Account and Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Account)
                .WithMany(a => a.Products)
                .HasForeignKey(p => p.AccountId);

            // Configure 1-N relationship between Product and ProductImage
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);

            // Configure 1-N relationship between ProductCategory and Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.CategoryId);

            // Configure N-N relationship between Color and Product
            modelBuilder.Entity<Color>()
                .HasMany(p => p.Products)
                .WithMany(c => c.Colors)
                .UsingEntity(j => j.ToTable("ProductColors"));

            // Configure N-N relationship between Size and Product
            modelBuilder.Entity<Size>()
                .HasMany(p => p.Products)
                .WithMany(s => s.Sizes)
                .UsingEntity(j => j.ToTable("ProductSizes"));

            // Configure N-N relationship between Material and Product
            modelBuilder.Entity<Material>()
                .HasMany(p => p.Products)
                .WithMany(m => m.Materials)
                .UsingEntity(j => j.ToTable("ProductMaterials"));

            // Configure N-N relationship between Delivery and Product
            modelBuilder.Entity<Delivery>()
                .HasMany(p => p.Products)
                .WithMany(d => d.Deliveries)
                .UsingEntity(j => j.ToTable("ProductDeliveries"));

            // Configure N-N relationship between Product and Tag
            modelBuilder.Entity<Tag>()
                .HasMany(t => t.Products)
                .WithMany(p => p.Tags)
                .UsingEntity(j => j.ToTable("ProductTags"));

            // Configure 1-1 relationship between Cart and Account
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Account)
                .WithOne(a => a.Cart)
                .HasForeignKey<Cart>(c => c.AccountId);

            // Configure 1-N relationship between Cart and CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            // Configure 1-N relationship between Product and CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Color and CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Color)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ColorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Delivery and CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Delivery)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.DeliveryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Material and CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Material)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.MaterialId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Size and CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Size)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.SizeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between CartItem and Design
            modelBuilder.Entity<Design>()
                .HasOne(ci => ci.CartItem)
                .WithMany(p => p.Designs)
                .HasForeignKey(ci => ci.CartItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Account and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId);

            // Configure 1-1 relationship between Order and OrderAddress
            modelBuilder.Entity<OrderAddress>()
                .HasOne(od => od.Order)
                .WithOne(o => o.OrderAddress)
                .HasForeignKey<OrderAddress>(od => od.OrderId);

            // Configure 1-N relationship between Order and OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            // Configure 1-N relationship between Product and OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Color and OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Color)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ColorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Delivery and OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Delivery)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.DeliveryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Material and OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Material)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.MaterialId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Size and OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Size)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.SizeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between OrderDetail and Design
            modelBuilder.Entity<Design>()
                .HasOne(od => od.OrderDetail)
                .WithMany(p => p.Designs)
                .HasForeignKey(od => od.OrderDetailId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Order and Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Product and Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure 1-N relationship between Review and ReviewImage
            modelBuilder.Entity<ReviewImage>()
                .HasOne(ri => ri.Review)
                .WithMany(r => r.ReviewImages)
                .HasForeignKey(ri => ri.ReviewId);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin" },
                new Role { Id = 2, RoleName = "Vendor" },
                new Role { Id = 3, RoleName = "Customer" }
            );

            modelBuilder.Entity<BlogCategory>().HasData(
                new BlogCategory { Id = 1, CategoryName = "Design Services" },
                new BlogCategory { Id = 2, CategoryName = "HaruTheme" },
                new BlogCategory { Id = 3, CategoryName = "Print Company" },
                new BlogCategory { Id = 4, CategoryName = "Print Shop" },
                new BlogCategory { Id = 5, CategoryName = "Uncategorized" }
            );

            modelBuilder.Entity<BlogTag>().HasData(
                new BlogTag { Id = 1, TagName = "Company" },
                new BlogTag { Id = 2, TagName = "Design Services" },
                new BlogTag { Id = 3, TagName = "HaruTheme" },
                new BlogTag { Id = 4, TagName = "Pricom" },
                new BlogTag { Id = 5, TagName = "Print" },
                new BlogTag { Id = 6, TagName = "Printing" },
                new BlogTag { Id = 7, TagName = "PrintShop" }
            );

            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 1, CategoryName = "Accessories" },
                new ProductCategory { Id = 2, CategoryName = "Apparel" },
                new ProductCategory { Id = 3, CategoryName = "Blankets" },
                new ProductCategory { Id = 4, CategoryName = "Canvas" },
                new ProductCategory { Id = 5, CategoryName = "Design Online" },
                new ProductCategory { Id = 6, CategoryName = "Poster" },
                new ProductCategory { Id = 7, CategoryName = "Uncategorized" }
            );

            modelBuilder.Entity<Color>().HasData(
                new Color { Id = 1, ThemeColor = "Black" },
                new Color { Id = 2, ThemeColor = "Black/Green" },
                new Color { Id = 3, ThemeColor = "Brown" },
                new Color { Id = 4, ThemeColor = "Red" },
                new Color { Id = 5, ThemeColor = "White" },
                new Color { Id = 6, ThemeColor = "White/Green" },
                new Color { Id = 7, ThemeColor = "Yellow" }
            );

            modelBuilder.Entity<Delivery>().HasData(
                new Delivery { Id = 1, DeliveryType = "Standard Shipping", ExtraFees = 0 },
                new Delivery { Id = 2, DeliveryType = "Express shipping", ExtraFees = 20 }
            );

            modelBuilder.Entity<Size>().HasData(
                new Size { Id = 1, ProductSize = "XS" },
                new Size { Id = 2, ProductSize = "S" },
                new Size { Id = 3, ProductSize = "M" },
                new Size { Id = 4, ProductSize = "L" },
                new Size { Id = 5, ProductSize = "XL" },
                new Size { Id = 6, ProductSize = "2XL" },
                new Size { Id = 7, ProductSize = "3XL" }
            );

            modelBuilder.Entity<Material>().HasData(
                new Material { Id = 1, MaterialType = "Glass" },
                new Material { Id = 2, MaterialType = "Metal" },
                new Material { Id = 3, MaterialType = "Paper" },
                new Material { Id = 4, MaterialType = "Wood" }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, TagName = "Banner" },
                new Tag { Id = 2, TagName = "Brochure" },
                new Tag { Id = 3, TagName = "Card" },
                new Tag { Id = 4, TagName = "Flyer" },
                new Tag { Id = 5, TagName = "Logo" },
                new Tag { Id = 6, TagName = "Packaging" },
                new Tag { Id = 7, TagName = "Poster" },
                new Tag { Id = 8, TagName = "Sticker" },
                new Tag { Id = 9, TagName = "T-Shirt" }
            );
        }
    }
}
