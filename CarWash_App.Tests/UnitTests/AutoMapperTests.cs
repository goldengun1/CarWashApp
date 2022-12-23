using AutoMapper;
using CarWash_App.Helpers;

namespace CarWash_App.Tests.UnitTests
{
    [TestClass]
    public class AutoMapperTests
    {
        [TestMethod]
        public void AssertConfigurationIsValid()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfiles>();
            });
            configuration.AssertConfigurationIsValid();
        }
    }
}
