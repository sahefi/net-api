using Microsoft.AspNetCore.Mvc;
using net_api.DTOs;
using net_api.Services;

namespace net_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(ApiResponse<List<TransactionResponse>>.Success(transactions, "Transactions retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<TransactionResponse>>.BadRequest(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null)
                {
                    return NotFound(ApiResponse<TransactionResponse>.NotFound("Transaction not found"));
                }

                return Ok(ApiResponse<TransactionResponse>.Success(transaction, "Transaction retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TransactionResponse>.BadRequest(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
        {
            try
            {
                // For demo, using hardcoded user ID. In production, get from JWT token
                var userId = 1;
                var transaction = await _transactionService.CreateTransactionAsync(request, userId);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, ApiResponse<TransactionResponse>.Created(transaction, "Transaction created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<TransactionResponse>.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TransactionResponse>.BadRequest(ex.Message));
            }
        }
    }
}
