using System;
using System.Collections.Generic;
using CleanArchitectureTest.Domain.Common;

namespace CleanArchitectureTest.Domain.Entities;

public partial class Role : BaseAuditableEntity
{
    public string Name { get; set; } = null!;

    public string Normalizedname { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
