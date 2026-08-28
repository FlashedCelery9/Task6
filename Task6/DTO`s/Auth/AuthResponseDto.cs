namespace Task6.DTO_s.Auth;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string TokenType);