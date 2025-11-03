namespace CleanArchitectureTest.Contract.Models.Auth;

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

