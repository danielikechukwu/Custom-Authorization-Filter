using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthorizationFilter.Filters
{
    // Custom authorization filter that checks user subscription level and expiry
    public class SubscriptionBasedAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] _allowSubscriptions;

        // Constructor accepting allowed subscription levels as params array
        public SubscriptionBasedAuthorizationFilter(params string[] allowedSubscriptions)
        {
            _allowSubscriptions = allowedSubscriptions;
        }

        // This method is called by the ASP.NET Core pipeline to authorize a request
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated; if not, respond with 401 Unauthorized and JSON error message
            if(!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = CreateJsonResponse(
                    401,                              // HTTP status code
                    "Unauthorized",                   // Error title
                    "Authentication is required to access this resource."  // Detailed message
                );

                return; // Stop further processing
            }

            // Retrieve the "SubscriptionLevel" claim value from the user's claims
            var subscriptionLevel = user.FindFirst("SubscriptionLevel")?.Value;

            // Retrieve the "SubscriptionExpiresOn" claim value (subscription expiration date)
            var subExpires = user.FindFirst("SubscriptionExpiresOn")?.Value;

            if(!_allowSubscriptions.Contains(subscriptionLevel))
            {
                context.Result = CreateJsonResponse(
                    403,                              // HTTP status code
                    "Forbidden",                      // Error title
                    $"Access requires one of the following subscription levels: {string.Join(", ", _allowSubscriptions)}. Your current level is '{subscriptionLevel}'." // Detailed message
                );

                return; // Stop further processing
            }

            if(subExpires != null && DateTime.TryParse(subExpires, out var expiresOn) && expiresOn < DateTime.UtcNow)
            {
                if(expiresOn < DateTime.UtcNow)
                {
                    context.Result = CreateJsonResponse(
                        403,                              // HTTP status code
                        "Subscription Expired",           // Error title
                        $"Your subscription expired on {expiresOn:d}. Please renew to regain access." // Detailed message
                    );
                    return; // Stop further processing
                }
            }
        }

        private JsonResult CreateJsonResponse(int statusCode, string error, string message)
        {
            // Create an anonymous object representing the JSON payload
            var jsonPayload = new
            {
                Status = statusCode,
                Error = error,
                Message = message
            };

            return new JsonResult(jsonPayload)
            {
                StatusCode = statusCode // Set the HTTP status code for the response
            };
        }
    }
}
