using Kernel.DTO.Request;
using Kernel.DTO.Response;
using Kernel.Model;
using Kernel.Model.Auth;
using Kernel.Util;

namespace Kernel.Controller;

public class AuthController
{
    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        await using var context = AuthContext.Get();
        
        var user = await User.GetUser(email: loginRequest.Email!);
        
        if(user is null)
            throw new Exception("User not found");
        
        if (loginRequest.Password != user.Password)
            throw new Exception("Invalid password");

        var expirationDate = DateTime.UtcNow.AddHours(6);
        
        return new LoginResponse
        {
            Actor = "user",
            Token = user!.GenerateJwt(expirationDate),
            ExpiresAt = expirationDate,
            Guid = user.Guid
        };
    }

    public async Task<VerifyResponse> VerifyToken()
    {
        await using var context = AuthContext.Get();
        return TokenManager.GetSubjectFromToken(Auth.GetToken()!);
    }
    
    public async Task<User?> GetUser(string? email, Guid? guid)
    {
        await VerifyToken();
        await using var context = AuthContext.Get();
        return await User.GetUser(email, guid);
    }
}