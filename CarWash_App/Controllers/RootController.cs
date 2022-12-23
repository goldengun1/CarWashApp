using CarWash_App.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace CarWash_App.Controllers
{
    [ApiController]
    [Route("api/")]
    public class RootController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public RootController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet(Name = "GetRoot")]
        public async Task<IEnumerable<Link>> Get()
        {
            List<Link> links = new List<Link>();

            links.Add(new Link(href: Url.Link("GetRoot", new { }), rel: "self", method: "GET"));
            links.Add(new Link(href: Url.Link("SignUp", new { }), rel: "Create_User", method: "POST"));
            links.Add(new Link(href: Url.Link("SignIn", new { }), rel: "User_LogIn", method: "POST"));
            links.Add(new Link(href: Url.Link("GetFiltered", new { }), rel: "Filter_CarWashes", method: "GET"));
            links.Add(new Link(href: Url.Link("ListAllServiceTypes", new { }), rel: "List_ServiceTypes", method: "GET"));

            if (User.Identity.IsAuthenticated)
            {
                links.Add(new Link(href: Url.Link("GetPersonal", new { }), rel: "Personal_Page", method: "GET"));
                
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Owner"))
            {
                var currUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var owner = context.applicationUsers.FirstOrDefaultAsync(x => x.Email.Equals(currUserEmail));

                links.Add(new Link(href: Url.Link("GetCarWash", new { }), rel: "Get_Car_Wash", method: "GET"));
                links.Add(new Link(href: Url.Link("GetCarWashServices", new { }), rel: "View_Services", method: "GET"));
                links.Add(new Link(href: Url.Link("GetCarWashStatistics", new { }), rel: "View_Revenue_Stats", method: "GET"));
                links.Add(new Link(href: Url.Link("CreateCarWash", new { }), rel: "Create_CarWash", method: "POST"));
                links.Add(new Link(href: Url.Link("DeleteAccount", new { email = currUserEmail }), rel: "Delete_Account", method: "DELETE"));

            }
            else if (User.Identity.IsAuthenticated && User.IsInRole("Customer"))
            {
                var currUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var customer = context.applicationUsers.FirstOrDefaultAsync(x => x.Email.Equals(currUserEmail));

                links.Add(new Link(href: Url.Link("ScheduleService", new { }), rel: "Schedule_Service", method: "POST"));
                links.Add(new Link(href: Url.Link("CancelService", new { }), rel: "Cancel_Service", method: "DELETE"));
                links.Add(new Link(href: Url.Link("DeleteAccount", new { email = currUserEmail }), rel: "Delete_Account", method: "DELETE"));
            }


            return links;
        }

    }
}
