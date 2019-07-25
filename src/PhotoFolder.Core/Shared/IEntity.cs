using System;

namespace PhotoFolder.Core.Shared
{
    public interface IEntity
    {
        DateTimeOffset CreatedOn { get; set; }
        DateTimeOffset ModifiedOn { get; set; }
    }

    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; }
    }
}
