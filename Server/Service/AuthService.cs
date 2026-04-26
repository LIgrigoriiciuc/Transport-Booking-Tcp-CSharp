using Server.Domain;
using Server.Repository;
using BC = BCrypt.Net.BCrypt;
namespace Server.Service;


public class AuthService : AbstractService<long, User>
{
    private const int BcryptRounds = 12;
    public User? LoggedInUser { get; private set; } = null;

    public AuthService(UserRepository repository) : base(repository)
    {
       
    }

    public static string HashPassword(string plainPassword)
        => BC.HashPassword(plainPassword, BcryptRounds);
 
    public static bool CheckPassword(string plainPassword, string hashed)
    {
        try { return BC.Verify(plainPassword, hashed); }
        catch { return false; }
    }
 
    public User Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Username and password are required.");
 
        var f = new Filter();
        f.AddFilter("username", username);
 
        var matches = Repository.Filter(f);
        if (!matches.Any())
            throw new UnauthorizedAccessException("Incorrect credentials.");
 
        var user = matches[0];
        string stored = user.Password;
        bool valid;
 
        if (stored.StartsWith("$2a$") || stored.StartsWith("$2b$") || stored.StartsWith("$2y$"))
        {
            valid = CheckPassword(password, stored);
        }
        else
        {
            valid = stored == password;
            if (valid)
            {
                user.Password = HashPassword(password);
                Repository.Update(user);
            }
        }
 
        if (!valid)
            throw new UnauthorizedAccessException("Incorrect credentials.");
 
        return user;
    }
}
