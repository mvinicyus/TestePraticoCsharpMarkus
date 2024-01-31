namespace Application.Boudary.User
{
    public class CreateUserOutput
    {
        public CreateUserOutput()
        {
        }
        public CreateUserOutput(string? name, string? email)
        {
            Name = name;
            Email = email;
        }

        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
