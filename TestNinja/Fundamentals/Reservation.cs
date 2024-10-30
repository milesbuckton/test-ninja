namespace TestNinja.Fundamentals
{
    public class Reservation
    {
        public User MadeBy { get; set; }

        public bool CanBeCancelledBy(User user) => user.IsAdmin || MadeBy == user;
    }

    public class User
    {
        public bool IsAdmin { get; set; }
    }
}
