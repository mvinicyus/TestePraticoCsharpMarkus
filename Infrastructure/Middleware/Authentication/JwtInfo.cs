namespace Infrastructure.Middleware.Authentication
{
    public class JwtInfo
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public long Id { get; set; }
    }
}
