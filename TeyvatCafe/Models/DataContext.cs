using Microsoft.EntityFrameworkCore;

namespace TeyvatCafe.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options): base(options) { }
        //DBSET
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountAddress> AccountAddresses { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductCart> ProductCarts { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Status> Statuses { get; set; }
        
        public DbSet<AccountStaff> AccountStaffs { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }
        public DbSet<Role> Roles { get; set; }


        //ModelCreation
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().ToTable("Account")
                .HasKey(p => p.IdAccount);
            modelBuilder.Entity<Attribute>().ToTable("Attribute")
                .HasKey(p => p.IdAttribute);
            modelBuilder.Entity<Category>().ToTable("Category")
                .HasKey(p=>p.IDCategory);
            modelBuilder.Entity<Product>().ToTable("Product")
                .HasKey(p => p.IdProduct);
            modelBuilder.Entity<ProductAttribute>().ToTable("ProductAttribute")
                .HasKey(p => new { p.IdAttribute, p.IdProduct });
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory")
                .HasKey(p => new { p.IdCategory, p.IdProduct });
            
                modelBuilder.Entity<Cart>().ToTable("Cart")
                .HasKey(p => p.IdCart);
            modelBuilder.Entity<ProductCart>().ToTable("ProductCart")
                .HasKey(p => new { p.IdCart,p.IdProduct });
            modelBuilder.Entity<Address>().ToTable("Address")
                .HasKey(p => p.IdAddress);
            modelBuilder.Entity<AccountAddress>().ToTable("AccountAddress")
                .HasKey(p => new { p.IdAddress, p.IdAccount });
            modelBuilder.Entity<AccountStaff>().ToTable("AccountStaff")
                .HasKey(p => new { p.IDStaff });
            modelBuilder.Entity<StaffRole>().ToTable("StaffRole")
                .HasKey(p => new { p.IDStaff, p.IdRole });
            modelBuilder.Entity<Invoice>().ToTable("Invoice")
                .HasKey(p => new { p.IdCart });
            modelBuilder.Entity<Status>().ToTable("Status")
                .HasKey(p => new { p.IdStatus });
            modelBuilder.Entity<Role>().ToTable("Role");

        }
    }
}
