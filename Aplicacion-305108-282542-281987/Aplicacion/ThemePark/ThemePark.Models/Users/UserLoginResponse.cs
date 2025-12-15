using ThemePark.Entities;

namespace ThemePark.Models.Users;

public class UserLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserWithAllResponse? Usuario { get; set; }

    public static UserLoginResponse FromEntity((Session Session, User User) loginResult)
    {
        return new UserLoginResponse
        {
            Token = loginResult.Session.Token,
            Usuario = UserWithAllResponse.FromEntity(loginResult.User)
        };
    }
}
