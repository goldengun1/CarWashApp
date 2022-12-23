using CarWash_App.Controllers;
using CarWash_App.DTOs.ServiceTypeDTOs;
using CarWash_App.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarWash_App.Tests.UnitTests
{
    [TestClass]
    public class ServiceTypesControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetServiceTypesFilteredOk()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            context.ServiceTypes.AddRange(serviceTypes);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

          
            //var loggerMock = new Mock<ILogger<ServiceTypesController>>();
            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();
            var filter = new ServiceTypeFilterDTO() { AvailableAnywhere = false };


            var controller = new ServiceTypesController(context2,mapper,loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Get(filter);

            var svctypes = response.Value;
            Assert.AreEqual(3, svctypes.Count);
        }

        [TestMethod]
        public async Task GetCoursesFilteredBadField()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            context.ServiceTypes.AddRange(serviceTypes);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();


            //var loggerMock = new Mock<ILogger<ServiceTypesController>>();
            var filter = new ServiceTypeFilterDTO() { AvailableAnywhere = false,OrderingField = "NonExistentField" };


            var controller = new ServiceTypesController(context2, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = await controller.Get(filter);

            var statuscode = response.Result as BadRequestObjectResult;
            Assert.AreEqual(400,statuscode.StatusCode);
        }

        [TestMethod]
        public async Task GetByIdExists()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            context.ServiceTypes.AddRange(serviceTypes);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();

            var controller = new ServiceTypesController(context2, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.GetById(1);
            var result = response.Value;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetByIdNotExists()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            context.ServiceTypes.AddRange(serviceTypes);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();

            var controller = new ServiceTypesController(context2, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.GetById(5);
            var statuscode = response.Result as StatusCodeResult;

            Assert.AreEqual(404,statuscode.StatusCode);
        }

        [TestMethod]
        public async Task CreateServiceType()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();
            var controller = new ServiceTypesController(context, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var serviceTypeCreation = new ServiceTypeCreationDTO()
            {
                ServiceName = "Test1",
                Duration = 1,
                Cost = 2.5f
            };

            var response = await controller.Post(serviceTypeCreation);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201,result.StatusCode);


            var context2 = BuildContext(databaseName);
            var count = await context2.ServiceTypes.CountAsync();
            Assert.AreEqual(1, count);

        }

        [TestMethod]
        public async Task DeleteSuccess()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            context.ServiceTypes.AddRange(serviceTypes);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();
            var controller = new ServiceTypesController(context2, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.Delete(1);
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            var count = await context3.ServiceTypes.CountAsync();
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task DeleteNotFound()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();
            var controller = new ServiceTypesController(context, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(404,result.StatusCode);
        }

        [TestMethod]
        public async Task PutCheckOk()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();

            var serviceTypes = new List<ServiceType>()
            {
                new ServiceType()
                {
                    Id = 1,
                    ServiceName = "Regular",
                    Duration = new TimeSpan(1,0,0),
                    Cost = 2.5f
                },
                new ServiceType()
                {
                    Id = 2,
                    ServiceName = "Extended",
                    Duration = new TimeSpan(2,0,0),
                    Cost = 4.5f
                },
                new ServiceType()
                {
                    Id = 3,
                    ServiceName = "Premium",
                    Duration = new TimeSpan(3,0,0),
                    Cost = 8.75f
                }
            };

            context.ServiceTypes.AddRange(serviceTypes);
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            var new_name = "Test1_NewName";
            var new_duration = 2;
            var new_cost = 3.5f;
            var serviceTypeCreation = new ServiceTypeCreationDTO()
            {
                ServiceName = new_name,
                Duration = new_duration,
                Cost = new_cost
            };

            var loggerMoq = Mock.Of<ILogger<ServiceTypesController>>();
            var controller = new ServiceTypesController(context2, mapper, loggerMoq);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.Put(1,serviceTypeCreation);
            var result = response as NoContentResult;
            Assert.AreEqual(204,result.StatusCode);

            var context3 = BuildContext(databaseName);

            var serviceType = await context3.ServiceTypes.FirstOrDefaultAsync(st => st.Id == 1);
            Assert.IsNotNull(serviceType);
            Assert.AreEqual(new_name, serviceType.ServiceName);
            Assert.AreEqual(new_duration, serviceType.Duration.Hours);
            Assert.AreEqual(new_cost, serviceType.Cost);
        }

    }
}

