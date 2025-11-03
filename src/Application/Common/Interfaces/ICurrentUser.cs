using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Id { get; }

    // Username (từ claim 'name' hoặc 'preferred_username')
    string? Username { get; }

    // Email (từ claim 'email')
    string? Email { get; }

    // Tên đầy đủ (từ claim 'given_name', 'full_name'...)
    string? FullName { get; }
    string? Phonenumber { get; }

    // Trạng thái đã xác thực
    bool IsAuthenticated { get; }

    // Danh sách các vai trò (roles) (từ claim 'role')
    IEnumerable<string> Roles { get; }

    // Một hàm helper để lấy claim bất kỳ nếu cần
    string? GetClaimValue(string claimType);

}
