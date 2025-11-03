using System;
using System.Collections.Generic;
using CleanArchitectureTest.Domain.Common;

namespace CleanArchitectureTest.Domain.Entities;

public partial class TodoItem : BaseAuditableEntity
{
    public Guid ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public int Priority { get; set; }

    public DateTimeOffset? Reminder { get; set; }

    public bool Done { get; set; }
}
