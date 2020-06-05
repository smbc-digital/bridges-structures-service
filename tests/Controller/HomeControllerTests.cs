using bridges_structures_service.Controllers;
using bridges_structures_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.AspNetCore.Availability.Managers;
using Xunit;

namespace bridges_structures_service_tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly Mock<IBridgesStructuresService> _mockBridgesStructuresService = new Mock<IBridgesStructuresService>();

        public HomeControllerTests()
        {
            _homeController = new HomeController(Mock.Of<ILogger<HomeController>>(), _mockBridgesStructuresService.Object);
        }       
    }
}
