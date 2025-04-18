namespace AuthUsingCustomAttribute.Application.DTOs;

public class TokenRequestDto
{
    public long UserId { get; set; }
    public int RoleId { get; set; }
}