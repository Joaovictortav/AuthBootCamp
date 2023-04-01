namespace Kernel.DTO.Response;

public class LoginResponse
{
    public string? Token { get; set; }
    public string Guid { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? Actor { get; set; }
}