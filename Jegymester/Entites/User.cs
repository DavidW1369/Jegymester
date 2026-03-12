namespace Jegymester.Entites
{
    public enum Role
    {
        User,
        RegisteredUser,
        Cashier,
        Admin
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public Role Role { get; set; }
    }
}
