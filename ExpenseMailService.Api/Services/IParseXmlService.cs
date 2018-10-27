using System.Xml.Linq;
using ExpenseMailService.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExpenseMailService.Api.Services
{
    public interface IParseXmlService
    {
        bool TryParseExpenseDto(string data, out ExpenseDto dto, ModelStateDictionary modelState);
        bool TryParseXmlDocument(string data, out XDocument document);
    }
}