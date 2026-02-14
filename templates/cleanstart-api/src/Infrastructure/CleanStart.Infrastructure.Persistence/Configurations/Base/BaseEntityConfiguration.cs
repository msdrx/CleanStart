using CleanStart.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanStart.Infrastructure.Persistence.Configurations.Base;

public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
      where TEntity : BaseEntity<TKey>
      where TKey : struct
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        if (typeof(TKey) == typeof(Guid) || typeof(TKey).IsEnum)
        {
            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
        }
        else
        {
            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        }

        ConfigureEntity(builder);

        builder.Property(x => x.CreateDate).HasColumnName("CreateDate").IsRequired();
    }

    public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
