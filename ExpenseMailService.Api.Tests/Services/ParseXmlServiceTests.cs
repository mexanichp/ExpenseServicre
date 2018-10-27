using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ExpenseMailService.Api.Tests.Services
{
    [TestFixture]
    public class ParseXmlServiceTests
    {
        private ILogger<ParseXmlService> _logger;
        private IConfiguration _configuration;

        private ParseXmlService _parseXmlService;

        [SetUp]
        public void SetUp()
        {
            _logger = Mock.Of<ILogger<ParseXmlService>>();
            _configuration = Mock.Of<IConfiguration>();

            _parseXmlService = new ParseXmlService(_logger, _configuration);
        }

        [TestCase("Hi Yvaine,\r\nPlease create an expense claim for the below. Relevant details are marked up as\r\nrequested…\r\n<expense><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\nFrom: Ivan Castle\r\nSent: Friday, 16 February 2018 10:32 AM\r\nTo: Antoine Lloyd <Antoine.Lloyd@example.com>\r\nSubject: test\r\nHi Antoine,\r\nPlease create a reservation at the <vendor>Viaduct Steakhouse</vendor> our\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>. We expect to arrive around\r\n7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.\r\nRegards,\r\nIvan")]
        [TestCase("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<root>\r\n<expense><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>\r\n</root>")]
        public void TryParseExpenseDto_InvokedWithProperlyFormattedXml_ShouldReturnTrue(string data)
        {
            // Arrange
            var configMock = Mock.Get(_configuration);
            configMock.Setup(t => t["XmlSettings:TotalXmlElementName"]).Returns("total");
            configMock.Setup(t => t["XmlSettings:CostCentreXmlElementName"]).Returns("cost_centre");
            configMock.Setup(t => t["XmlSettings:XmlDeclaration"]).Returns("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            configMock.Setup(t => t["XmlSettings:XmlRootElementTag"]).Returns("root");

            // Act
            var res = _parseXmlService.TryParseExpenseDto(data, out var dto, new ModelStateDictionary());

            // Assert
            Assert.IsTrue(res);
            Assert.AreEqual(1024.01m, dto.Total);
        }

        [TestCase("Hi Yvaine,\r\nPlease create an expense claim for the below. Relevant details are marked up as\r\nrequested…\r\n<expense><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\nFrom: Ivan Castle\r\nSent: Friday, 16 February 2018 10:32 AM\r\nTo: Antoine Lloyd <Antoine.Lloyd@example.com>\r\nSubject: test\r\nHi Antoine,\r\nPlease create a reservation at the <vendor>Viaduct Steakhouse</vendor> our\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>. We expect to arrive around\r\n7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.\r\nRegards,\r\nIvan")]
        [TestCase("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<root>\r\n<expense><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>\r\n</root>")]
        public void TryParseExpenseDto_InvokedWithNoTotalXmlElement_ShouldReturnFalse(string data)
        {
            // Arrange
            var configMock = Mock.Get(_configuration);
            configMock.Setup(t => t["XmlSettings:TotalXmlElementName"]).Returns("total1");
            configMock.Setup(t => t["XmlSettings:CostCentreXmlElementName"]).Returns("cost_centre");
            configMock.Setup(t => t["XmlSettings:XmlDeclaration"]).Returns("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            configMock.Setup(t => t["XmlSettings:XmlRootElementTag"]).Returns("root");

            // Act
            var res = _parseXmlService.TryParseExpenseDto(data, out var dto, new ModelStateDictionary());

            // Assert
            Assert.IsFalse(res);
            Assert.IsNull(dto);
        }

        [Test]
        public void TryParseExpenseDto_InvokedOneOfTagsDoesNotHaveClosingElement_ShouldReturnFalse()
        {
            // Arrange
            var configMock = Mock.Get(_configuration);
            configMock.Setup(t => t["XmlSettings:TotalXmlElementName"]).Returns("total");
            configMock.Setup(t => t["XmlSettings:CostCentreXmlElementName"]).Returns("cost_centre");
            configMock.Setup(t => t["XmlSettings:XmlDeclaration"]).Returns("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            configMock.Setup(t => t["XmlSettings:XmlRootElementTag"]).Returns("root");
            var data = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<root>\r\n<expense2><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>\r\n</root>";

            // Act
            var res = _parseXmlService.TryParseExpenseDto(data, out var dto, new ModelStateDictionary());

            // Assert
            Assert.IsFalse(res);
            Assert.IsNull(dto);
        }

        [Test]
        public void TryParseExpenseDto_ConfigurationIsMissingRootElementTag_ShouldReturnFalse()
        {
            // Arrange
            var configMock = Mock.Get(_configuration);
            configMock.Setup(t => t["XmlSettings:TotalXmlElementName"]).Returns("total");
            configMock.Setup(t => t["XmlSettings:CostCentreXmlElementName"]).Returns("cost_centre");
            configMock.Setup(t => t["XmlSettings:XmlDeclaration"]).Returns("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            configMock.Setup(t => t["XmlSettings:XmlRootElementTag"]).Returns(null as string);
            var data = "Hi Yvaine,\r\nPlease create an expense claim for the below. Relevant details are marked up as\r\nrequested…\r\n<expense><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\nFrom: Ivan Castle\r\nSent: Friday, 16 February 2018 10:32 AM\r\nTo: Antoine Lloyd <Antoine.Lloyd@example.com>\r\nSubject: test\r\nHi Antoine,\r\nPlease create a reservation at the <vendor>Viaduct Steakhouse</vendor> our\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>. We expect to arrive around\r\n7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.\r\nRegards,\r\nIvan";

            // Act
            var res = _parseXmlService.TryParseExpenseDto(data, out var dto, new ModelStateDictionary());

            // Assert
            Assert.IsFalse(res);
            Assert.IsNull(dto);
        }
    }
}