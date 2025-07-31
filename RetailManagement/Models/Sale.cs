using System;

namespace RetailManagement.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public string BillNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsCredit { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
    }
} 