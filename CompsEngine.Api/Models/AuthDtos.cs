public record RegisterRequest(string Email, string UserName, string Password);
public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
