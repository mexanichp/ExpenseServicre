using System;
using System.Threading.Tasks;
using ExpenseMailService.Api.Controllers;
using ExpenseMailService.Api.Models;
using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ExpenseMailService.Api.Tests.Controllers
{
    [TestFixture]
    public class ExpensesControllerTests : ControllerTestsBase
    {
        private ExpensesController _controller;
        private IParseXmlService _parseXmlService;

        [SetUp]
        public void SetUp()
        {
            _parseXmlService = Mock.Of<IParseXmlService>();
            _controller = new ExpensesController(_parseXmlService);
            MockRequest(_controller);
        }

        [Test]
        public async Task Post_ModelStateIsNotValid_ReturnsBadRequest()
        {
            // Arrange

            _controller.ModelState.AddModelError("", "");

            // Act
            var res = await _controller.Post(new ExpenseInputDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(res);
        }

        [Test]
        public async Task Post_Invoked_ReturnsOkResult()
        {
            // Arrange
            var expenseDto = new ExpenseDto();
            Mock.Get(_parseXmlService)
                .Setup(t => t.TryParseExpenseDto(It.IsAny<string>(), out expenseDto, It.IsAny<ModelStateDictionary>()))
                .Returns(true);

            // Act
            var res = await _controller.Post(new ExpenseInputDto());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(res);
        }

        [Test]
        public async Task Post_Invoked_ReturnsOkResultWithCorrectExpenseDto()
        {
            // Arrange
            var expenseDto = new ExpenseDto();
            Mock.Get(_parseXmlService)
                .Setup(t => t.TryParseExpenseDto(It.IsAny<string>(), out expenseDto, It.IsAny<ModelStateDictionary>()))
                .Returns(true);

            // Act
            var res = await _controller.Post(new ExpenseInputDto {Data = "any data"}) as OkObjectResult;
            var resultModel = res?.Value as ExpenseDto;

            // Assert
            Assert.IsNotNull(resultModel);
        }
    }
}