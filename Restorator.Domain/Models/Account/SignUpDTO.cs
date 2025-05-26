namespace Restorator.Domain.Models.Account
{
    public class SignUpDTO
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
    }
}
