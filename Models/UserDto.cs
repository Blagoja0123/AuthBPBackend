namespace Auth.Models
{
    public class UserDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        public string? Base64PhotoString { get; set; }
    }
}
