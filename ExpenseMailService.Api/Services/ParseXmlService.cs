using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ExpenseMailService.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExpenseMailService.Api.Services
{
    public class ParseXmlService : IParseXmlService
    {
        private readonly ILogger<ParseXmlService> _logger;
        private readonly IConfiguration _configuration;

        public ParseXmlService(ILogger<ParseXmlService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public bool TryParseExpenseDto(string data, out ExpenseDto dto, ModelStateDictionary modelState)
        {
            dto = null;

            if (TryParseXmlDocument(data, out var document))
            {
                dto = ParseXDocumentToExpenseDto(document, ref modelState);
                if (dto == null)
                {
                    return false;
                }

                return true;
            }

            try
            {
                var openingTags = Regex.Matches(data, @"(?<=<)([A-z_0-9]*)(?=>)", RegexOptions.Singleline);
                var openingTagsWithoutClosingTags = openingTags.Where(ot => !Regex.IsMatch(data, $"</{ot.Value}>")).ToList();
                if (openingTagsWithoutClosingTags.Any())
                {
                    modelState.AddModelError("Closing tag does not exist for the following tags", string.Join("; ", openingTagsWithoutClosingTags));
                    return false;
                }

                var xmlLikeContent = Regex.Matches(data, @"<([A-z_0-9]*)>(.*)<\/\1>", RegexOptions.Singleline);
                var xmlDynamicContent = _configuration["XmlSettings:XmlDeclaration"]
                                        + $"<{_configuration["XmlSettings:XmlRootElementTag"]}>"
                                        + string.Join('\n', xmlLikeContent.Select(t => t.Value))
                                        + $"</{_configuration["XmlSettings:XmlRootElementTag"]}>";

                if (!TryParseXmlDocument(xmlDynamicContent, out document))
                {
                    throw new ArgumentException("Data was not properly formatted or configuration is missing.", nameof(data));
                }

                dto = ParseXDocumentToExpenseDto(document, ref modelState);
                if (dto == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                dto = null;
                _logger.LogError(e, $"Error occured under parsing XML service. Data - {data}");
                modelState.AddModelError("Error", "Server was unable to parse passed XML.");
                return false;
            }
        }

        public bool TryParseXmlDocument(string data, out XDocument document)
        {
            try
            {
                document = XDocument.Parse(data);
                return true;
            }
            catch
            {
                document = null;
                return false;
            }
        }


        private ExpenseDto ParseXDocumentToExpenseDto(XDocument document, ref ModelStateDictionary modelState)
        {
            var allElements = document.Descendants().ToList();
            var totalXElement = allElements.SingleOrDefault(t => t.Name == _configuration["XmlSettings:TotalXmlElementName"]);
            if (totalXElement == null || !decimal.TryParse(totalXElement.Value, out var total))
            {
                modelState.AddModelError(nameof(ExpenseDto.Total), "Total amount is missing or has inappropriate format.");
                return null;
            }

            var expenseDto = new ExpenseDto {Total = total};
            var costCentreXElement = allElements.SingleOrDefault(t => t.Name == _configuration["XmlSettings:CostCentreXmlElementName"]);
            if (costCentreXElement != null && !costCentreXElement.IsEmpty)
            {
                expenseDto.CostCentre = costCentreXElement.Value;
            }

            expenseDto.Data = document.Root;
            return expenseDto;
        }
    }
}