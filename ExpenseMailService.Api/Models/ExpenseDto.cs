namespace ExpenseMailService.Api.Models
{
    /// <summary>
    /// Expense object.
    /// </summary>
    public class ExpenseDto
    {
        /// <summary>
        /// GST based on Total.
        /// </summary>
        public string GST => (Total * 3 / 23).ToString("F");

        /// <summary>
        /// Total amount with no GST.
        /// </summary>
        public string TotalWithoutGST => (Total - Total * 3 / 23).ToString("F");

        /// <summary>
        /// Cost Centre if exists.
        /// </summary>
        public string CostCentre { get; set; } = "UNKNOWN";

        /// <summary>
        /// Total amount of cost.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Dynamically parsed XML content.
        /// </summary>
        public object Data { get; set; }
    }
}