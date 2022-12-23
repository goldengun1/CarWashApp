using CarWash_App.Controllers;
using CarWash_App.DTOs.ServiceDTOs;
using CarWash_App.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarWash_App.Tests.UnitTests
{
    [TestClass]
    public class ServicesControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetServiceAsCustomerOk()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<ServicesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultCustomer();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);

            var controller = new ServicesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.Get(1);
            var result = response.Value;
            Assert.IsNotNull(result);
            Assert.AreEqual(1,result.Id);

        }

        [TestMethod]
        public async Task GetServiceAsOwnerOk()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<ServicesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);

            var controller = new ServicesController(context2, mapper, httpContextAccessor, loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.Get(1);
            var result = response.Value;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);

        }

        [TestMethod]
        public async Task ScheduleServiceOk()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<ServicesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultCustomer();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var serviceScheduling = new ServiceCreationDTO()
            {
                CarWashId = 1,
                ServiceTypeId = 1,
                ScheduledTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,13,0,0),
            };
            var controller = new ServicesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.Post(serviceScheduling);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var responseFromDB = await context3.Services.ToListAsync();
            Assert.IsNotNull(responseFromDB);
            Assert.AreEqual(2, responseFromDB.Count);
        }

        [TestMethod]
        public async Task ScheduleServiceNotAvailable()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<ServicesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultCustomer();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var serviceScheduling = new ServiceCreationDTO()
            {
                CarWashId = 1,
                ServiceTypeId = 1,
                ScheduledTime = DateTime.Now,
            };
            var controller = new ServicesController(context2, mapper, httpContextAccessor, loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.Post(serviceScheduling);
            var result = response as BadRequestObjectResult;
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Unable to schedule service at desired date/time", result.Value);
        }

        [TestMethod]
        public async Task CancelServiceEligibleTrue()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<ServicesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultCustomer();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var controller = new ServicesController(context2, mapper, httpContextAccessor, loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.CancelService(1);
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var count = await context3.Services.CountAsync();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task CancelServiceEligibleFalse()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<ServicesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultCustomer();
            SeedTestData(context);
            var serviceForCanceling = new Service()
            {
                Id = 2,
                ServiceTypeId = 1,
                CarWashId = 1,
                CustomerId = "22D1E20A-0E46-4425-9480-329E46821EC7",
                ScheduledTime = DateTime.Now.AddHours(2),
                EligibleForCancelation = false
            };
            context.Services.Add(serviceForCanceling);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var controller = new ServicesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.CancelService(2);
            var result = response as BadRequestObjectResult;
            Assert.AreEqual(400,result.StatusCode);
        }
    }
}
