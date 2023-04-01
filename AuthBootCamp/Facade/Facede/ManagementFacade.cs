using Kernel.Controller;
using Kernel.DTO.Request;
using Kernel.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace AuthBootCamp.Facede;


[ApiController]    
[Route("V1/Management", Name = "Management")]	
[Produces("application/json")]
public class ManagementFacade : FacadeBase
{
    public ManagementFacade() 
	{
	}

    [HttpPost, Route("CreateUser")] 
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)] 
	public async Task<IActionResult> CreateUser([FromBody] UserRequest userRequest) 
	{ 
		try 
		{
			object ret = await new ManagementController().CreateUser(userRequest); 
			return Json(ret); 
		} 
		catch (Exception e)
		{
			throw new Exception($"Erro: {e.Message}");
		} 
	}

	[HttpPost, Route("UpdateUser")] 
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)] 
	public async Task<IActionResult> UpdateUser([FromBody] UserRequest user) 
	{
		try 
		{
			object ret = await new ManagementController().UpdateUser(user); 
			return Json(ret); 
		} 
		catch (Exception e) 
		{ 
			throw new Exception($"Erro: {e.Message}");
		} 
	}
}