using AestusDemoAPI.Domain.Entitites;
using Microsoft.EntityFrameworkCore;


namespace AestusDemoAPI.Infrastructure
{
    public class FinTechAestusContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; } = null!;

        public FinTechAestusContext(DbContextOptions<FinTechAestusContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>().HasData(
              new Transaction
              {
                  Id = 1,
                  UserId = "user_001",
                  Amount = 50.00,
                  Timestamp = DateTime.Parse("2024-04-24T09:00:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 2,
                  UserId = "user_001",
                  Amount = 49.50,
                  Timestamp = DateTime.Parse("2024-04-24T09:05:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 3,
                  UserId = "user_001",
                  Amount = 5000.00,
                  Timestamp = DateTime.Parse("2024-04-24T09:07:00Z"),
                  Location = "Ljubljana",
                  IsSuspicious = true,
                  Comment = "Neuobičajeno visok iznos + neočekivana lokacija"
              },
              new Transaction
              {
                  Id = 4,
                  UserId = "user_001",
                  Amount = 48.00,
                  Timestamp = DateTime.Parse("2024-04-24T09:20:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 5,
                  UserId = "user_001",
                  Amount = 47.99,
                  Timestamp = DateTime.Parse("2024-04-24T09:30:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 6,
                  UserId = "user_001",
                  Amount = 4000.00,
                  Timestamp = DateTime.Parse("2024-04-24T09:32:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = true,
                  Comment = "Neuobičajeno visok iznos"
              },
              new Transaction
              {
                  Id = 7,
                  UserId = "user_001",
                  Amount = 50.10,
                  Timestamp = DateTime.Parse("2024-04-24T09:40:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 8,
                  UserId = "user_001",
                  Amount = 52.00,
                  Timestamp = DateTime.Parse("2024-04-24T09:50:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 9,
                  UserId = "user_001",
                  Amount = 51.00,
                  Timestamp = DateTime.Parse("2024-04-24T09:55:00Z"),
                  Location = "Zagreb",
                  IsSuspicious = false,
                  Comment = "Normalna potrošnja"
              },
              new Transaction
              {
                  Id = 10,
                  UserId = "user_001",
                  Amount = 49.99,
                  Timestamp = DateTime.Parse("2024-04-24T10:00:00Z"),
                  Location = "Beograd",
                  IsSuspicious = true,
                  Comment = "Neočekivana lokacija"
              }
          );
        }
    }
}
