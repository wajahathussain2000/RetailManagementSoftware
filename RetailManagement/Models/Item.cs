using System;

namespace RetailManagement.Models
{
    public class Item
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
} 