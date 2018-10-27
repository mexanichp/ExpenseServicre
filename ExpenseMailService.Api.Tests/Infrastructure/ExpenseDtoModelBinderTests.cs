using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExpenseMailService.Api.Infrastructure;
using ExpenseMailService.Api.Models;
using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ExpenseMailService.Api.Tests.Infrastructure
{
    [TestFixture]
    public class ExpenseDtoModelBinderTests
    {
        private Mock<ModelBindingContext> _context;
        private Mock<IParseXmlService> _parseXmlService;
        private IConfiguration _configuration;

        private ExpenseDtoModelBinder _binder;

        [SetUp]
        public void SetUp()
        {
            _configuration = Mock.Of<IConfiguration>();
            _parseXmlService = new Mock<IParseXmlService>();
            _context = new Mock<ModelBindingContext>();
            _binder = new ExpenseDtoModelBinder(_parseXmlService.Object);
        }


        [Test]
        public async Task BindModelAsync_InvokedWithFailedParsing_ModelShouldBeNull()
        {
            // Arrange
            var assertPayload = "<test>test</test>";
            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();

            httpRequest.Setup(t => t.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(assertPayload)));
            httpContext.Setup(t => t.Request).Returns(httpRequest.Object);
            _context.Setup(t => t.HttpContext).Returns(httpContext.Object);
            _context.Setup(t => t.ModelState).Returns(new ModelStateDictionary());

            ExpenseDto outVar;
            _parseXmlService
                .Setup(t => t.TryParseExpenseDto(It.IsAny<string>(), out outVar, It.IsAny<ModelStateDictionary>()))
                .Returns(false);

            // Act
            await _binder.BindModelAsync(_context.Object);

            // Assert
            _context.VerifySet(t => t.Result = It.Is<ModelBindingResult>(r => r.Model == null && !r.IsModelSet));
        }

        [Test]

        public async Task BindModelAsync_Invoked_ModelShouldBeExpenseDto()

        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();

            httpRequest.Setup(t => t.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes("<test></test>")));
            httpContext.Setup(t => t.Request).Returns(httpRequest.Object);
            _context.SetupAllProperties();
            _context.Setup(t => t.HttpContext).Returns(httpContext.Object);

            ExpenseDto outVar = new ExpenseDto();
            _parseXmlService
                .Setup(t => t.TryParseExpenseDto(It.IsAny<string>(), out outVar, It.IsAny<ModelStateDictionary>()))
                .Returns(true);

            // Act
            await _binder.BindModelAsync(_context.Object);

            // Assert
            _context.VerifySet(t => t.Result = It.Is<ModelBindingResult>(r => r.Model is ExpenseDto), Times.Once);
        }
    }
}
