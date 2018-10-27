using System;
using System.Threading.Tasks;
using ExpenseMailService.Api.Controllers;
using ExpenseMailService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace ExpenseMailService.Api.Tests.Controllers
{
    [TestFixture]
    public class ExpensesControllerTests : ControllerTestsBase
    {
        private ExpensesController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new ExpensesController();
            MockRequest(_controller);
        }

        [Test]
        public async Task Post_ModelStateIsNotValid_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("", "");

            // Act
            var res = await _controller.Post(new ExpenseDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(res);
        }

        [Test]
        public async Task Post_Invoked_ReturnsOkResult()
        {
            // Arrange

            // Act
            var res = await _controller.Post(new ExpenseDto());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(res);
        }

        [Test]
        public async Task Post_Invoked_ReturnsOkResultWithCorrectExpenseDto()
        {
            // Arrange
            var dto = new ExpenseDto {Total = 204m, CostCentre = Guid.NewGuid().ToString(), Data = new object()};

            // Act
            var res = await _controller.Post(dto) as OkObjectResult;
            var resultModel = res?.Value as ExpenseDto;

            // Assert
            Assert.IsNotNull(resultModel);
            Assert.AreEqual(dto.Total, resultModel.Total);
            Assert.AreEqual(dto.CostCentre, resultModel.CostCentre);
            Assert.AreEqual(dto.Data, resultModel.Data);
            Assert.AreEqual(dto.GST, resultModel.GST);
            Assert.AreEqual(dto.TotalWithoutGST, resultModel.TotalWithoutGST);
            Assert.AreEqual(dto.Data, resultModel.Data);
        }
    }
}