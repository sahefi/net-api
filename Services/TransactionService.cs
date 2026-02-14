using Microsoft.EntityFrameworkCore;
using net_api.Data;
using net_api.DTOs;
using net_api.Models;

namespace net_api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionResponse>> GetAllTransactionsAsync()
        {
            var transactions = await _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Product)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(MapToResponse).ToList();
        }

        public async Task<TransactionResponse?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Product)
                .FirstOrDefaultAsync(t => t.Id == id);

            return transaction == null ? null : MapToResponse(transaction);
        }

        public async Task<TransactionResponse> CreateTransactionAsync(TransactionRequest request, int userId)
        {
            // Using transaction for atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get all product IDs
                var productIds = request.Items.Select(i => i.ProductId).ToList();

                // Lock products for update to prevent concurrency issues
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                // Validate all products exist
                var missingProducts = productIds.Except(products.Select(p => p.Id)).ToList();
                if (missingProducts.Any())
                {
                    throw new InvalidOperationException($"Products with IDs {string.Join(", ", missingProducts)} not found");
                }

                // Check stock for OUT transactions
                if (request.Type == TransactionType.OUT)
                {
                    foreach (var item in request.Items)
                    {
                        var product = products.First(p => p.Id == item.ProductId);
                        if (product.Stock < item.Quantity)
                        {
                            throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Required: {item.Quantity}");
                        }
                    }
                }

                // Create transaction
                var newTransaction = new Transaction
                {
                    Type = request.Type,
                    ReferenceNumber = GenerateReferenceNumber(),
                    Notes = request.Notes,
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.Transactions.Add(newTransaction);
                await _context.SaveChangesAsync();

                // Create transaction details and update stock
                foreach (var item in request.Items)
                {
                    var product = products.First(p => p.Id == item.ProductId);

                    var detail = new TransactionDetail
                    {
                        TransactionId = newTransaction.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        SubTotal = product.Price * item.Quantity
                    };

                    _context.TransactionDetails.Add(detail);

                    // Update stock based on transaction type
                    if (request.Type == TransactionType.IN)
                    {
                        product.Stock += item.Quantity;
                    }
                    else // OUT
                    {
                        product.Stock -= item.Quantity;
                    }

                    product.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return the created transaction with details
                var createdTransaction = await _context.Transactions
                    .Include(t => t.TransactionDetails)
                    .ThenInclude(td => td.Product)
                    .FirstAsync(t => t.Id == newTransaction.Id);

                return MapToResponse(createdTransaction);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string GenerateReferenceNumber()
        {
            return $"TRX-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{new Random().Next(1000, 9999)}";
        }

        private static TransactionResponse MapToResponse(Transaction transaction)
        {
            return new TransactionResponse
            {
                Id = transaction.Id,
                Type = transaction.Type,
                ReferenceNumber = transaction.ReferenceNumber,
                TransactionDate = transaction.TransactionDate,
                Notes = transaction.Notes,
                Items = transaction.TransactionDetails.Select(td => new TransactionItemResponse
                {
                    ProductId = td.ProductId,
                    ProductName = td.Product?.Name ?? "Unknown",
                    Quantity = td.Quantity,
                    UnitPrice = td.UnitPrice,
                    SubTotal = td.SubTotal
                }).ToList()
            };
        }
    }
}
