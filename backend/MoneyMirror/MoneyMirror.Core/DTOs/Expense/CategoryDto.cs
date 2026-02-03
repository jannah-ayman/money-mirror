namespace MoneyMirror.Core.DTOs.Expense
{
    /// <summary>
    /// Data Transfer Object for returning a category.
    /// Used for dropdowns in the frontend.
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Category ID.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Category name.
        /// Example: "Snacks / Food"
        /// </summary>
        public string Name { get; set; }
    }
}