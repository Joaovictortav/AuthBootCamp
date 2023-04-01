using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kernel.DTO.Request;
using Kernel.Util;
using Microsoft.EntityFrameworkCore;

namespace Kernel.Model.Auth
{
	[Table("users")]
	public sealed class User
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("ID", TypeName = "INT")] public int Id { get; set; } 
		[Column("name", TypeName = "VARCHAR(255)"), MaxLength(255), Required] public string? Name { get; set; } 
		[Column("password", TypeName = "VARCHAR(255)"), MaxLength(255), Required] public string? Password { get; set; } 
		[Column("email", TypeName = "VARCHAR(100)"), MaxLength(100), Required] public string? Email { get; set; } 
		[Column("guid", TypeName = "VARCHAR(36)"), Required] public string Guid { get; set; } 

		public User() { }
		public User (UserRequest user)
		{
			Name = user.Name;
			Email = user.Email;
			Password = user.Password!;
			Guid = new Guid().ToString();

			AuthContext.Get().UserSet.Add(this);
		}
		internal void Delete()
		{
			AuthContext.Get().UserSet.Remove(this);
		}
		
		internal string GenerateJwt(DateTime? expireDate)
		{
			return TokenManager.GenerateJwt(Guid.ToString(), Email, Name, expireDate);
		}
		
		public static async Task<User?> GetUser(string? email = null, Guid? guid = null)
		{
			IQueryable<User> set = AuthContext.Get().UserSet;
			bool isNull = true;

			if (!string.IsNullOrEmpty(email))
			{
				set = set.Where(s => s.Email == email);
				isNull = false;
			}

			if (guid is not null)
			{
				set = set.Where(s => s.Guid == guid.ToString());
				isNull = false;
			}

			return isNull ? null : await set.FirstOrDefaultAsync();
		}

		public Task<User> Update(UserRequest user)
		{
			Name = user.Name;
			Password = user.Password!;

			return Task.FromResult(this);
		}
	}
}