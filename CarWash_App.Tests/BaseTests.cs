using AutoMapper;
using CarWash_App.Entities;
using CarWash_App.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace CarWash_App.Tests
{
    public class BaseTests
    {
        protected ApplicationDbContext BuildContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName).Options;

            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }
        protected IMapper BuildMap()
        {
            var config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfiles());
            });
            return config.CreateMapper();
        }

        protected ControllerContext BuildControllerContextWithDefaultCustomer()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,"customer.customer"),
                    new Claim(ClaimTypes.Email, "test@testmail.com"),
                }, "test"));

            return new ControllerContext() { HttpContext = new DefaultHttpContext() { User = user } };
        }

        protected ControllerContext BuildControllerContextWithDefaultOwner()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,"mihailo.simic"),
                    new Claim(ClaimTypes.Email, "mihailo.simic@gmail.com"),
                }, "test"));

            return new ControllerContext() { HttpContext = new DefaultHttpContext() { User = user } };
        }

        protected void SeedTestData(ApplicationDbContext context)
        {
            //DATA
            var owner = new ApplicationUser()
            {
                Id = "FBB5FC51-4270-48C1-BADD-A441FF5759F3",
                FirstName = "Mihailo",
                LastName = "Simic",
                IsAnOwner = true,
                UserName = "mihailo.simic",
                NormalizedUserName = "MIHAILO.SIMIC",
                Email = "mihailo.simic@gmail.com",
                NormalizedEmail = "MIHAILO.SIMIC@GMAIL.COM",
                EmailConfirmed = false,
                PasswordHash = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890!@#$%^&*()_+"
            };
            var customer = new ApplicationUser()
            {
                Id = "22D1E20A-0E46-4425-9480-329E46821EC7",
                FirstName = "Customer",
                LastName = "Customer",
                IsAnOwner = false,
                UserName = "customer.customer",
                NormalizedUserName = "CUSTOMER.CUSTOMER",
                Email = "test@testmail.com",
                NormalizedEmail = "TEST@TESTMAIL.COM",
                EmailConfirmed = false,
                PasswordHash = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890!@#$%^&*()_+"
            };
            var carWash = new CarWash()
            {
                Id = 1,
                CarWashName = "CARWASH1",
                OwnerId = "FBB5FC51-4270-48C1-BADD-A441FF5759F3",
                OpeningTime = 9,
                ClosingTime = 17
            };
            var service = new Service()
            {
                Id = 1,
                CustomerId = "22D1E20A-0E46-4425-9480-329E46821EC7",
                CarWashId = 1,
                ServiceTypeId = 1,
                ScheduledTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 9, 0, 0)

            };
            var serviceType = new ServiceType()
            {
                ServiceName = "Regular",
                Duration = TimeSpan.FromHours(1),
                Cost = 2.5f
            };
            //END_DATA
            context.Users.Add(owner);
            context.Users.Add(customer);
            context.carWashes.Add(carWash);
            context.Services.Add(service);
            context.ServiceTypes.Add(serviceType);
            context.SaveChanges();
        }

        protected HttpContext BuildHttpContextWithDefaultCustomer()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
               {
                    new Claim(ClaimTypes.Name,"customer.customer"),
                    new Claim(ClaimTypes.Email, "test@testmail.com"),
               }, "test"));

            return new DefaultHttpContext() 
            {
                User = user,
                Connection =
                {
                    RemoteIpAddress = new System.Net.IPAddress(16885952)
                }
            };
        }

        protected HttpContext BuildHttpContextWithDefaultOwner()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,"mihailo.simic"),
                    new Claim(ClaimTypes.Email, "mihailo.simic@gmail.com"),
                }, "test"));

            return new DefaultHttpContext()
            {
                User = user,
                Connection =
                {
                    RemoteIpAddress = new System.Net.IPAddress(16885952),
                    RemotePort = 30000,
                    LocalIpAddress = new System.Net.IPAddress(1111)
                }
            };
        }

    }
}
