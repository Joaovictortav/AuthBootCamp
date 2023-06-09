using Kernel.DTO.Request;
using Kernel.DTO.Response;
using Kernel.Model;
using Kernel.Model.Auth;

namespace Kernel.Controller;

public class ManagementController
{
    public async Task<bool> UpdateUser(UserRequest userRequest)
    {
        await using var context = AuthContext.Get(); 
        var user = await User.GetUser(email: userRequest.Email!);
			
        if (user is null)
            throw new Exception("User not found");
			
        await user.Update(userRequest);
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> CreateUser(UserRequest userRequest)
    {
        await using var context = AuthContext.Get(); 
        var user = await User.GetUser(email: userRequest.Email!);     
        
        if (user is not null)
            throw new Exception("User already registered in database");

        _ = new User(userRequest);
        
        await context.SaveChangesAsync();
        return true;  
    }
}