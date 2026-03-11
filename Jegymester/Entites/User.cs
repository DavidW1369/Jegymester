namespace Jegymester.Entites
{
    public class User
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public enum Role
        {
            User,
            RegisteredUser,
            Cashier,
            Admin
        }
    }
}
