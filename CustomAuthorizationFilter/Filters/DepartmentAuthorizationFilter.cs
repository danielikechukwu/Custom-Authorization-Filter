using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomAuthorizationFilter.Filters
{
    public class DepartmentAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string _allowedDepartments;

        public DepartmentAuthorizationFilter(string allowedDepartments)
        {
            _allowedDepartments = allowedDepartments;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
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

            var department = user.FindFirst("Department")?.Value;

            // Check if department claim is missing or does NOT match the allowed department (case-insensitive)
            if (department == null || !string.Equals(department, _allowedDepartments, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = CreateJsonResponse(
                    403,
                    "Forbidden",
                    $"Access requires membership in one of the following departments: {string.Join(", ", _allowedDepartments)}. Your current department is '{department}'."
                );

                return;
            }

            await Task.CompletedTask;
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
