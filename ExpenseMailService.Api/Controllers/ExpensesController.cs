using System.Threading.Tasks;
using ExpenseMailService.Api.Models;
using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseMailService.Api.Controllers
{
    [Route("api/[controller]")]
    public class ExpensesController : Controller
    {
        private readonly IParseXmlService _parseXmlService;

        public ExpensesController(IParseXmlService parseXmlService)
        {
            _parseXmlService = parseXmlService;
        }

        // POST api/expenses
        /// <summary>
        /// Parse XML-like content to the Expense model.
        /// </summary>
        /// <remarks>
        /// Returns 400 Bad request if XML content is not valid or Total amount is missing in input.
        /// </remarks>
        /// <param name="data">XML-like content</param>
        /// <returns>Returns parsed object with GMT calculated.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExpenseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ExpenseInputDto data)
        {
            if (!ModelState.IsValid || !_parseXmlService.TryParseExpenseDto(data.Data, out var dto, ModelState))
            {
                return BadRequest(ModelState);
            }

            return Ok(dto);
        }
    }
}
