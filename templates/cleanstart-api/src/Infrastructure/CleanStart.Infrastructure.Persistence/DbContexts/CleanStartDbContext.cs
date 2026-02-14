using CleanStart.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;

namespace CleanStart.Infrastructure.Persistence.DbContexts;

public class CleanStartDbContext : DbContext
{

    ///TODO: add DbSets here

    public CleanStartDbContext(DbContextOptions<CleanStartDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Core");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseEntityConfiguration<,>).Assembly);

        //singular table names
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.DisplayName().ToUpper());
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
        {
            property.SetColumnName(property.Name.ToUpper());
        }

        //disable cascade delete for all entities
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        base.OnModelCreating(modelBuilder);
    }
}
