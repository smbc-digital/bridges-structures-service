using bridges_structures_service.Controllers;
using bridges_structures_service.Models;
using bridges_structures_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.AspNetCore.Availability.Managers;
using System.Threading.Tasks;
using Xunit;

namespace bridges_structures_service_tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController _homeController;
        private readonly Mock<IBridgesStructuresService> _mockBridgesStructuresService = new Mock<IBridgesStructuresService>();

        public HomeControllerTest()
        {
            _homeController = new HomeController(Mock.Of<ILogger<HomeController>>(), _mockBridgesStructuresService.Object);
        }

        [Fact]
        public async Task Post_ShouldCallCreateCase()
        {
            _mockBridgesStructuresService
                .Setup(_ => _.CreateCase(It.IsAny<BridgesStructuresReport>()))
                .ReturnsAsync("test");

            var result = await _homeController.Post(null);

            _mockBridgesStructuresService
                .Verify(_ => _.CreateCase(null), Times.Once);
        }

        [Fact]
        public async Task Post_ReturnOkActionResult()
        {
            _mockBridgesStructuresService
                .Setup(_ => _.CreateCase(It.IsAny<BridgesStructuresReport>()))
                .ReturnsAsync("test");

            var result = await _homeController.Post(null);

            Assert.Equal("OkObjectResult", result.GetType().Name);
        }
    }
}
