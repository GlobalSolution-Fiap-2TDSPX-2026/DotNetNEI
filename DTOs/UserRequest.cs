namespace NEI;

public record class UserRequest(
    string Username,
    string Email,
    Role Role
)
{
    public User ToEntity()
    {
        return new User
        {
            Username=this.Username,
            Email=this.Email,
            Role=this.Role,
        };
    }
}
