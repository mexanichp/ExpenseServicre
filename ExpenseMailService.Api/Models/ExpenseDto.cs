using ExpenseMailService.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseMailService.Api.Models
{
    [ModelBinder(typeof(ExpenseDtoModelBinder))]
    public class ExpenseDto
    {
        public string GST => (Total * 3 / 23).ToString("F");

        public string TotalWithoutGST => (Total - Total * 3 / 23).ToString("F");

        public string CostCentre { get; set; } = "UNKNOWN";

        public decimal Total { get; set; }

        public object Data { get; set; }
    }
}