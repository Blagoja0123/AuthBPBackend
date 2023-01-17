namespace Auth.Models
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public byte[]? Photo { get; set; }
    }
}
