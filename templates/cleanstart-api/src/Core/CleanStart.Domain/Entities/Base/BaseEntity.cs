using CleanStart.Domain.Enums;
using CleanStart.Domain.Exceptions;

namespace CleanStart.Domain.Entities.Base;

public abstract class BaseEntity<TKey> : IBaseEntity where TKey : struct
{
    public TKey Id { get; set; }
    public DateTimeOffset CreateDate { get; set; }

    public void Create()
    {
        CreateDate = DateTimeOffset.Now;
    }
}

public abstract class BaseSoftDeleteEntity<TKey> : BaseEntity<TKey>, IBaseSoftDeleteEntity where TKey : struct
{
    public RecordStatus RecordStatus { get; set; }
    public string CreateUserId { get; set; } = string.Empty;

    public void Create(string createUserId)
    {
        base.Create();
        RecordStatus = RecordStatus.Active;
        CreateUserId = createUserId;
    }

    public void Activate()
    {
        if (IsActive()) throw DomainException.Create("Entity is already active");

        RecordStatus = RecordStatus.Active;
    }

    public void Delete()
    {
        if (IsDeleted()) throw DomainException.Create("Entity is already deleted");

        RecordStatus = RecordStatus.Deleted;
    }

    public bool IsDeleted() => RecordStatus == RecordStatus.Deleted || RecordStatus == RecordStatus.DeletedByParent;
    public bool IsActive() => RecordStatus == RecordStatus.Active;
}

