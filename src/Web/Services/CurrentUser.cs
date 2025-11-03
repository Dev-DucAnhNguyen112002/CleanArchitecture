using System.Security.Claims;

using CleanArchitectureTest.Application.Common.Interfaces;

namespace CleanArchitectureTest.Web.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    // Trạng thái
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    // Lấy ID từ NameIdentifier (chuẩn của .NET)
    public string? Id => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    // Lấy Username từ Name (chuẩn của .NET)
    public string? Username => User?.FindFirstValue(ClaimTypes.Name);

    // Lấy Email từ Email (chuẩn của .NET)
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    // Lấy FullName (đây thường là claim custom,
    // có thể dùng GivenName hoặc một claim tên "fullName" bạn tự định nghĩa)
    public string? FullName => User?.FindFirstValue(ClaimTypes.GivenName);
    public string? Phonenumber => User?.FindFirstValue(ClaimTypes.MobilePhone);

    // Lấy tất cả các claim có loại là Role
    public IEnumerable<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

    public string? GetClaimValue(string claimType) =>
        User?.FindFirstValue(claimType);
}
