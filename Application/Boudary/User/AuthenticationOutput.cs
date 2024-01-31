namespace Application.Boudary.User
{
    public class AuthenticationOutput
    {
        public string JwtToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Login { get; set; }
        public string? Name { get; set; }
    }
}
