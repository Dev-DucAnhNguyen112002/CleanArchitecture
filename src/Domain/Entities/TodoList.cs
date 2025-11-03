using System;
using System.Collections.Generic;
using CleanArchitectureTest.Domain.Common;

namespace CleanArchitectureTest.Domain.Entities;

public partial class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }

    public string? ColourCode { get; set; }
}
