using bridges_structures_service.Config;
using bridges_structures_service.Helpers;
using bridges_structures_service.Models;
using bridges_structures_service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Threading.Tasks;
using Xunit;

namespace bridges_structures_service_tests.Services
{
    public class BridgesStructuresServiceTests
    {
        private Mock<IVerintServiceGateway> _mockVerintServiceGateway = new Mock<IVerintServiceGateway>();
        private BridgesStructuresService _service;
        private Mock<ILogger<BridgesStructuresService>> _mocklogger = new Mock<ILogger<BridgesStructuresService>>();
        private Mock<IOptions<BridgesStructuresListConfiguration>> _mockBridgesStructuresConfig = new Mock<IOptions<BridgesStructuresListConfiguration>>();
        private Mock<IMailHelper> _mockMailHelper = new Mock<IMailHelper>();

        BridgesStructuresReport _bridgesStructuresReportData = new BridgesStructuresReport
        {
            FirstName = "Joe",
            LastName = "Bloggs",
            Email = "joe@test.com",
            Phone = "0161 123 1234",
            Details = "test details",
            GeneralEnquiry = "General details",
            FurtherInformation = "Further details",
            StructureAffected = "bridge",
            TypeOfRequest = "roadTrafficAccident",
            StreetAddress = new StockportGovUK.NetStandard.Models.Addresses.Address
            {
                PlaceRef = "123456",
                SelectedAddress = "green lane"
            }          
        };
        IConfiguration config = InitConfiguration();
        
        public BridgesStructuresServiceTests()
        {
            _service = new BridgesStructuresService(_mockVerintServiceGateway.Object,
                config,
                _mocklogger.Object,
                bridgesStructuresConfiguration(),
                _mockMailHelper.Object);
        }

        [Fact]
        public async Task CreateCase_ShouldReThrowCreateCaseException_CaughtFromVerintGateway()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Throws(new Exception("TestException"));

            var result = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_bridgesStructuresReportData));
            Assert.Contains($"CRMService CreateBridgesOrStructuresService an exception has occured while creating the case in verint service", result.Message);
        }

        [Fact]
        public async Task CreateCase_ShouldThrowException_WhenIsNotSuccessStatusCode()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = false
                });

            _ = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(_bridgesStructuresReportData));
        }

        [Fact]
        public async Task CreateCase_ShouldReturnResponseContent()
        {

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var result = await _service.CreateCase(_bridgesStructuresReportData);

            Assert.Contains("test", result);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCRMCase()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            _ = await _service.CreateCase(_bridgesStructuresReportData);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Equal(_bridgesStructuresReportData.StreetAddress.PlaceRef, crmCaseParameter.Street.Reference);
            Assert.Contains(_bridgesStructuresReportData.FurtherInformation, crmCaseParameter.Description);
            Assert.Contains(_bridgesStructuresReportData.Details, crmCaseParameter.Description);
        }

        [Theory]
        [InlineData("gantry", "generalEnquiry", "Bridges & Structures >> Gantry >> General Enquiry", "2003089")]
        [InlineData("tunnel", "roadTrafficAccident", "Bridges & Structures >> Tunnel >> RTA", "2003056")]
        [InlineData("steps", "safetyIssue", "Bridges & Structures >> Steps >> Safety Issue - General", "2003053")]
        [InlineData("footbridge", "roadTrafficAccident", "Bridges & Structures >> Bridge >> RTA", "2003046")]
        [InlineData("culvert", "roadTrafficAccident", "Bridges & Structures >> Culvert >> RTA", "2003048")]
        public async Task CreateCase_ShouldCallServiceWithCorrectEventCodeAndClassification(string structureAffected, string typeOfRequest, string classification, string eventCode)
        {
            var report = _bridgesStructuresReportData;
            report.StructureAffected = structureAffected;
            report.TypeOfRequest = typeOfRequest;        
            
            Case caseRequest = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => caseRequest = _)
                .ReturnsAsync(new HttpResponse<string>
                {
                    IsSuccessStatusCode = true
                });

            await _service.CreateCase(report);

            Assert.NotNull(caseRequest);
            Assert.Equal(classification, caseRequest.Classification);
            Assert.Equal(eventCode, caseRequest.EventCode.ToString());
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        public static IOptions<BridgesStructuresListConfiguration> bridgesStructuresConfiguration()
        {
            var collection = new ServiceCollection();
            collection.AddOptions();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", optional: false)
                .Build();

            collection.Configure<BridgesStructuresListConfiguration>(config.GetSection("BridgesStructuresConfiguration"));
           
            var services = collection.BuildServiceProvider();

            var options = services.GetService<IOptions<BridgesStructuresListConfiguration>>();
            
            return options;
        }        
    }
}
