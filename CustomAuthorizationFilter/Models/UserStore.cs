namespace CustomAuthorizationFilter.Models
{
    public class UserStore
    {
        public static List<User> Users = new List<User>
        {
            new User {Id = 1, Email ="Alice@Example.com",  Password = "alice123",
                SubscriptionLevel = "Pro", SubscriptionExpiresOn = DateTime.UtcNow.AddDays(5), Department = "HR" },

            new User {Id = 2, Email ="Bob@Example.com",  Password = "bob123",
                 SubscriptionLevel = "Free", Department = "IT"  },

            new User {Id = 3, Email ="Charlie@Example.com", Password = "charlie123",
                SubscriptionLevel = "Premium", SubscriptionExpiresOn = DateTime.UtcNow.AddDays(-2), Department = "HR"  },

             new User {Id = 4, Email ="eve@Example.com", Password = "eve123",
                SubscriptionLevel = "Premium", SubscriptionExpiresOn = DateTime.UtcNow.AddDays(30), Department = "Sales"  }
        };
    }
}
