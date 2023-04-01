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
		[Column("Name", TypeName = "VARCHAR(255)"), MaxLength(255), Required] public string? Name { get; set; } 
		[Column("Password", TypeName = "VARCHAR(255)"), MaxLength(255), Required] public string? Password { get; set; } 
		[Column("Gender", TypeName = "VARCHAR(1)"), MaxLength(1)] public string? Gender { get; set; } 
		[Column("BirthDate", TypeName = "DATETIME")] public DateTime? BirthDate { get; set; } 
		[Column("Email", TypeName = "VARCHAR(100)"), MaxLength(100), Required] public string? Email { get; set; } 
		[Column("Guid", TypeName = "Char(36)"), Required] public Guid Guid { get; set; } 
		[Column("Cellphone", TypeName = "VARCHAR(11)"), MaxLength(11)] public string? Cellphone { get; set; } 
		[Column("HomePhone", TypeName = "VARCHAR(11)"), MaxLength(11)] public string? HomePhone { get; set; }

		public User() { }
		public User (UserRequest user)
		{
			Name = user.Name;
			Email = user.Email;
			Password = user.Password!;
			Cellphone = user.Cellphone;
			HomePhone = user.HomePhone;
			Gender = user.Gender;
			BirthDate = user.BirthDate;
			Guid = Guid.NewGuid();

			AuthContext.Get().UserSet.Add(this);
		}
		internal void Delete()
		{
			AuthContext.Get().UserSet.Remove(this);
		}
		
		internal string GenerateJwt(DateTime? expireDate)
		{
			return TokenManager.GenerateJwt(Guid.ToString(), Email, Name,Cellphone, expireDate);
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
				set = set.Where(s => s.Guid == guid.Value);
				isNull = false;
			}

			return isNull ? null : await set.FirstOrDefaultAsync();
		}

		public Task<User> Update(UserRequest user)
		{
			Name = user.Name;
			Cellphone = user.Cellphone;
			HomePhone = user.HomePhone;
			Password = user.Password!;
			Gender = user.Gender;
			BirthDate = user.BirthDate;

			return Task.FromResult(this);
		}
	}
}