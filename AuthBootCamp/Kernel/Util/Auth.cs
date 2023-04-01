namespace Kernel.Util;

public abstract class Auth : IDisposable
{
    private static readonly AsyncLocal<string?> Token = new();
    public static string? SetToken(string token)
    {
        if (Token.Value! is not null)
            throw new Exception($"Token has already been set");

        Token!.Value = token;
        
        return Token.Value;
    }
    internal static string? GetToken()
    {
        if (Token.Value is null)
            throw new Exception("Token not found");
        
        return Token.Value;
    }

    public virtual void Dispose()
    {
        Token.Value = null;
    }
}