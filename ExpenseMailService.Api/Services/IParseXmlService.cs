using System.Xml.Linq;
using ExpenseMailService.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExpenseMailService.Api.Services
{
    /// <summary>
    /// Service for parsing XML or XML-like content.
    /// </summary>
    public interface IParseXmlService
    {
        /// <summary>
        /// Parse string data into <see cref="ExpenseDto"/> object.
        /// </summary>
        /// <param name="data">XML content with raw XML or any XML-like tags.</param>
        /// <param name="dto">Out parsed model.</param>
        /// <param name="modelState">ModelState of Controller to set up errors.</param>
        /// <returns>Returns true if parsing is successful otherwise false.</returns>
        bool TryParseExpenseDto(string data, out ExpenseDto dto, ModelStateDictionary modelState);

        /// <summary>
        /// Parse string data to XDocument.
        /// </summary>
        /// <param name="data">XML content.</param>
        /// <param name="document">Out parsed document.</param>
        /// <returns>Returns true if parsing is successful otherwise false.</returns>
        bool TryParseXmlDocument(string data, out XDocument document);
    }
}