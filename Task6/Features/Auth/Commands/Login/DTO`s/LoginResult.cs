using Task6.DTO_s.Auth;

namespace Task6.Features.Auth.Commands.Login.DTO_s;

public record LoginResult(bool Success, bool IsLockedOut, AuthResponseDto? Response);
