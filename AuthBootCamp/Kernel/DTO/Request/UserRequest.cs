namespace Kernel.DTO.Request;

public class UserRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Gender { get; set; }
    public string? Cellphone { get; set; }
    public string? HomePhone { get; set; }
    public DateTime BirthDate { get; set; }
    public string? Email { get; set; }
}