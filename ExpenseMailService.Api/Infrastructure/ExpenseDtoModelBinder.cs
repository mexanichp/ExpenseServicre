using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExpenseMailService.Api.Infrastructure
{
    public class ExpenseDtoModelBinder : IModelBinder
    {
        private readonly IParseXmlService _parseXmlService;

        public ExpenseDtoModelBinder(IParseXmlService parseXmlService)
        {
            _parseXmlService = parseXmlService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;

            string body;
            using (var receiveStream = request.Body)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    body = await readStream.ReadToEndAsync();
                    body = body.Trim();
                }
            }

            if (_parseXmlService.TryParseExpenseDto(body, out var dto, bindingContext.ModelState))
            {
                bindingContext.Result = ModelBindingResult.Success(dto);
                return;
            }

            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}