using AuthUsingCustomAttribute.Domain.Enums;

namespace AuthUsingCustomAttribute.Application.Attributes;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRolesAttribute : Attribute
{
    public UserRoleEnum[] Roles { get; }

    public AuthorizeRolesAttribute(params UserRoleEnum[] roles)
    {
        Roles = roles;
    }
}