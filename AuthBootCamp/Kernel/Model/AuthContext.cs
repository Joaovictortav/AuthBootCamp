using Kernel.Model.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Kernel.Model;

public class AuthContext : DbContext
{
    private static AsyncLocal<AuthContext> _instance = new ();
    
    internal static AuthContext Get()
    {
        if (_instance.Value is null)
        {
            var options = new DbContextOptions<AuthContext>();
            _instance.Value = new AuthContext(options);
        }
        
        return _instance.Value;
    }
    
    public DbSet<User> UserSet { get; set; }
    internal static void ResetContext(){
        _instance.Value = null!;
    }

    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            var connectionString = configuration.GetConnectionString("MySql");
            optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.27-mysql"));
        }
    }
    
    public override void Dispose()
    {
        base.Dispose();
        _instance.Value = null!;
    }

}