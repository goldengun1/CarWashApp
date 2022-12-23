using CarWash_App.Controllers;
using CarWash_App.DTOs.CarWashDTOs;
using CarWash_App.DTOs.ServiceDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarWash_App.Tests.UnitTests
{
    [TestClass]
    public class CarWashesControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetMonthlyRevenueNoPayments()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context3 = BuildContext(databaseName);
            var ss = context3.Services.First();
            ss.ScheduledTime = DateTime.Today.AddDays(2);
            context3.SaveChanges();

            var context2 = BuildContext(databaseName);
            var filter = new CarWashRevenueFilter() { Monthly = true};
            var controller = new CarWashesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultOwner();

            var response = await controller.GetRevenue(1, filter);
            var result = response.Result as OkObjectResult;
            Assert.AreEqual(200,result.StatusCode);
            Assert.AreEqual("No payments collected in the last month.",result.Value);
        }

        [TestMethod]
        public async Task GetMonthlyRevenueHasPayments()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var filter = new CarWashRevenueFilter() { Monthly = true };
            var controller = new CarWashesController(context2, mapper, httpContextAccessor, loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultOwner();

            var response = await controller.GetRevenue(1, filter);
            var result = response.Result as OkObjectResult;
            var code = response.Result as StatusCodeResult;
            Assert.AreEqual("No payments collected in the last month.", result.Value.ToString());
            //Assert.IsNull(code);
        }

        [TestMethod]
        public async Task GetCarWashServicesPermissionDenied()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultCustomer();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var controller = new CarWashesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultCustomer();

            var response = await controller.GetServices(1);
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(403,result.StatusCode);
        }

        [TestMethod]
        public async Task GetCarWashStatsNoServices()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context3 = BuildContext(databaseName);
            var ss = context3.Services.First();
            ss.ScheduledTime = DateTime.Today.AddDays(2);
            context3.SaveChanges();

            var context2 = BuildContext(databaseName);
            var controller = new CarWashesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultOwner();


            var response = await controller.GetCarWashStats(1);
            var result = response.Value;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalScheduled);
            Assert.AreEqual(0, result.CofirmedServices);
        }

        [TestMethod]
        public async Task ConfirmServiceOK()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var controller = new CarWashesController(context2,mapper,httpContextAccessor,loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultOwner();

            var response = await controller.ConfirmService(new DTOs.ServiceDTOs.ServiceConfirmationDTO() {CarWashId = 1, ServiceId = 1 });
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public async Task ConfirmAlredyConfirmed()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var svc = context2.Services.First();
            svc.Confirmed = true;
            context2.Services.Update(svc);
            context2.SaveChanges();

            var context3 = BuildContext(databaseName);
            var controller = new CarWashesController(context3, mapper, httpContextAccessor, loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultOwner();

            var response = await controller.ConfirmService(new ServiceConfirmationDTO() { CarWashId = 1, ServiceId = 1});
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
        }

        [TestMethod]
        public async Task RejectServiceUnableTimeRestriction()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMoq = Mock.Of<ILogger<CarWashesController>>();
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            httpContextAccessor.HttpContext = BuildHttpContextWithDefaultOwner();
            SeedTestData(context);

            var context2 = BuildContext(databaseName);
            var svc = context2.Services.First();
            svc.ScheduledTime = DateTime.Now.AddMinutes(30);
            context2.Services.Update(svc);
            context2.SaveChanges();

            var context3 = BuildContext(databaseName);
            var controller = new CarWashesController(context3, mapper, httpContextAccessor, loggerMoq);
            controller.ControllerContext = BuildControllerContextWithDefaultOwner();

            var response = await controller.RejectBookedService(new ServiceConfirmationDTO() { CarWashId = 1, ServiceId = 1});
            var result = response as BadRequestObjectResult;
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Unable to cancel service that is less than 1h from now.",result.Value);
        }
    }
}
