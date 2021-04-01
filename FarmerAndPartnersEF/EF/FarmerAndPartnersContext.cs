namespace FarmerAndPartnersEF.EF
{
    using FarmerAndPartnersModels;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class FarmerAndPartnersContext : DbContext
    {
        internal FarmerAndPartnersContext() { }
        public FarmerAndPartnersContext(DbContextOptions options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContractStatus> ContractStatuses { get; set; }

        public string GetTableName(Type type) => Model.FindEntityType(type).GetTableName();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                var connectionString = @"Server=(LocalDb)\MSSQLLocalDB;Database=FarmerAndPartnersCore;Trusted_Connection=True;";
#else
                var connectionString = @"Server=.\SQLEXPRESS;Database=FarmerAndPartnersCore;Trusted_Connection=True;";
#endif
                optionsBuilder.UseSqlServer(connectionString, option => option.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasOne(c => c.ContractStatus)
                .WithMany(c => c.Companies)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
