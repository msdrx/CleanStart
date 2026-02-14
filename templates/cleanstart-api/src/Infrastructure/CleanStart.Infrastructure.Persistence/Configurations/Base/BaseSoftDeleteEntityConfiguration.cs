using CleanStart.Domain.Entities.Base;
using CleanStart.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanStart.Infrastructure.Persistence.Configurations.Base;

public abstract class BaseSoftDeleteEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
      where TEntity : BaseSoftDeleteEntity<TKey>
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

        builder.Property(x => x.RecordStatus).HasColumnName("RecordStatus").IsRequired();
        builder.Property(x => x.CreateDate).HasColumnName("CreateDate").IsRequired();
        builder.Property(x => x.CreateUserId).HasColumnName("CreateUserId").HasMaxLength(100).IsRequired();

        builder.HasIndex(x => x.RecordStatus);

        //global query filter for ISoftDelete entity
        builder.HasQueryFilter(x => x.RecordStatus == RecordStatus.Active);
    }

    public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
