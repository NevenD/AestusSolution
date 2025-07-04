using AestusDemoAPI.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AestusDemoAPI.Infrastructure.Configuration
{
    public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId);
            builder.HasIndex(x => x.UserId).HasDatabaseName("transactions_userId");
            builder.Property(x => x.Amount);
            builder.Property(x => x.Timestamp);
            builder.HasIndex(x => x.Timestamp).HasDatabaseName("transactions_timestamp");
            builder.Property(x => x.Location);
            builder.Property(x => x.IsSuspicious);
            builder.Property(x => x.Comment);
        }
    }
}
