using CustomAuthorizationFilter.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthorizationFilter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        // Endpoint protected by subscription-based filter requiring "Premium" or "Pro"
        [HttpGet("premium-analytics")]
        [TypeFilter(typeof(SubscriptionBasedAuthorizationFilter), Arguments = new object[] { new[] {"Premium", "Pro"}})]
        public IActionResult GetPremiumAnalytics()
        {
            return Ok(new { message = "Welcome to the Premium Analytics section!" });
        }

        // Endpoint restricted to users from the "HR" department only
        [HttpGet("salary-review")]
        // [ApiExplorerSettings(IgnoreApi = true)] // --- To keep out of swagger docs ---
        [TypeFilter(typeof(DepartmentAuthorizationFilter), Arguments = new object[] { "HR" })]
        public IActionResult GetSalaryReview()
        {
            return Ok(new { message = "Welcome to the Salary Review section for HR!" });
        }

        // Endpoint accessible only during specified business hours (9 AM - 6 PM UTC)
        [HttpGet("business-hours-report")]
        [BusinessHoursAuthorize(9, 18)] // Access allowed only between 9 AM and 6 PM
        public IActionResult GetBusinessHoursReport()
        {
            return Ok(new { message = "Welcome to the Business Hours Report section!" });
        }
    }
}
