using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CustomAuthorizationFilter.Filters
{
    public class BusinessHoursAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        private readonly int _startHour; // Start of allowed hours (inclusive)

        private readonly int _endHour; // End of allowed hours (exclusive)

        public BusinessHoursAuthorizeAttribute(int startHour = 9, int endHour = 18)
        {
            _startHour = startHour;
            _endHour = endHour;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get current user principal from the HTTP context
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = CreateJsonResponse(
                    401,
                    "Unauthorized",
                    "Authentication is required to access this resource."
                );

                return;
            }

            var now = DateTime.Now.TimeOfDay;

            // Check if current hour is outside the allowed business hours range
            if (now.Hours < _startHour || now.Hours >= _endHour)
            {
                // Return 403 Forbidden with JSON error message for restricted access time
                context.Result = CreateJsonResponse(
                    403,
                    "Forbidden",
                    $"Access is only allowed between {_startHour}:00 and {_endHour}:00. Current time is {now.Hours}:{now.Minutes:D2}."
                );

                return;
            }

            // If user is authenticated and current time is within business hours, request proceeds normally

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
