using System;

namespace EcommerceAPI.Entities.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; } // EF will generate on Add
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
