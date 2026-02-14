using System.ComponentModel.DataAnnotations;
using net_api.Models;

namespace net_api.DTOs
{
    public class TransactionItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class TransactionRequest
    {
        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [MinLength(1)]
        public List<TransactionItemRequest> Items { get; set; } = new();

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class TransactionItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class TransactionResponse
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public List<TransactionItemResponse> Items { get; set; } = new();
    }
}
