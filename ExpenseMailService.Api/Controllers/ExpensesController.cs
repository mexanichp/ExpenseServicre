using System.Threading.Tasks;
using ExpenseMailService.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseMailService.Api.Controllers
{
    [Route("api/[controller]")]
    public class ExpensesController : Controller
    {
        // POST api/expenses
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] ExpenseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(dto);
        }
    }
}
