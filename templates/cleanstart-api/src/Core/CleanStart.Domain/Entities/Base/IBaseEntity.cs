using CleanStart.Domain.Enums;

namespace CleanStart.Domain.Entities.Base;
public interface IBaseEntity
{
    DateTimeOffset CreateDate { get; }
}

public interface ISoftDeleteEntity
{
    RecordStatus RecordStatus { get; set; }
}

public interface ICreatedByUserEntity
{
    string CreateUserId { get; }
}

public interface IBaseSoftDeleteEntity : IBaseEntity, ICreatedByUserEntity, ISoftDeleteEntity
{
}



