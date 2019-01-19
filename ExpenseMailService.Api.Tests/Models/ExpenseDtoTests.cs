using System;
using System.Collections.Generic;
using System.Text;
using ExpenseMailService.Api.Models;
using NUnit.Framework;

namespace ExpenseMailService.Api.Tests.Models
{
    [TestFixture]
    public class ExpenseDtoTests
    {
        private ExpenseDto _model;

        [SetUp]
        public void SetUp()
        {
            _model = new ExpenseDto {Data = new object()};
        }

        [Test]
        public void GST_Get_ShouldReturnCorrectData()
        {
            // Arrange
            const decimal total = 200;
            var expectedGst = (total * 3 / 23).ToString("F");
            _model.Total = total;

            // Act
            var result = _model.GST;

            // Assert
            Assert.AreEqual(expectedGst, result);
        }

        [Test]
        public void TotalWithoutGst_Get_ShouldReturnCorrectData()
        {
            // Arrange
            const decimal total = 200;
            var expectedTotalWithoutGst = (total - total * 3 / 23).ToString("F");
            _model.Total = total;

            // Act
            var result = _model.TotalWithoutGST;

            // Assert
            Assert.AreEqual(expectedTotalWithoutGst, result);
        }

        [Test]
        public void CostCenter_NotSet_ShouldReturnUnknownByDefault()
        {
            // Arrange

            // Act

            // Assert
            Assert.AreEqual("UNKNOWN", _model.CostCentre);
        }
    }
}
