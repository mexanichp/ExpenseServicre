using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ExpenseMailService.Api.Models
{
    /// <summary>
    /// Input model.
    /// </summary>
    public class ExpenseInputDto
    {
        /// <summary>
        /// XML-like or XML content.
        /// </summary>
        [JsonProperty("xml-like-content")]
        [Required]
        public string Data { get; set; }
    }
}