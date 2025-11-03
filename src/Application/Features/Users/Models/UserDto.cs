using System;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.Users.Models;

public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; set; } = null!;

    public string Normalizedusername { get; set; } = null!;

    public string? Email { get; set; }

    public string? Normalizedemail { get; set; }

    public bool Emailconfirmed { get; set; }

    public string Passwordhash { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public bool Phonenumberconfirmed { get; set; }

    public bool Twofactorenabled { get; set; }

    public DateTimeOffset? Lockoutend { get; set; }

    public bool Lockoutenabled { get; set; }

    public int Accessfailedcount { get; set; }

    public bool? IsActive { get; set; }
    public string? FullName { get; set; }

    public DateTimeOffset Created { get; init; }
    public string? CreatedBy { get; init; }
    public DateTimeOffset? LastModified { get; init; }
    public string? LastModifiedBy { get; init; }
}

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}
