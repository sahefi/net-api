using net_api.DTOs;

namespace net_api.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> GetAllTransactionsAsync();
        Task<TransactionResponse?> GetTransactionByIdAsync(int id);
        Task<TransactionResponse> CreateTransactionAsync(TransactionRequest request, int userId);
    }
}
