namespace CustomAuthorizationFilter.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SubscriptionLevel { get; set; } = null!; // "Free", "Premium", "Pro"
        public DateTime? SubscriptionExpiresOn { get; set; }
        public string Department { get; set; } = null!;
    }
}
