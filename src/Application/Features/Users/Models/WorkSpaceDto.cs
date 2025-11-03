using System;
using System.Collections.Generic;

namespace ISAT.Application.Features.Users.Models;

public record WorkspaceItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsFolder { get; set; }
    public bool IsShared { get; set; }
    public string PermissionTypeCode { get; set; } = "admin"; // view | edit | admin
    public Guid? ParentFolderId { get; set; }
    public string? Description { get; set; }
    public string? OriginalName { get; set; }
    public string? FileExtension { get; set; }
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
    public string? StoragePath { get; set; }
    public string? BucketName { get; set; }
    public string? LocationGeoserver { get; set; }
    public DateTimeOffset? CaptureDate { get; set; }

    public bool IsFavorited { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
