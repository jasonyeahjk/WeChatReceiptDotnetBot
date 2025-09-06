using WeChatReceiptBot.API.DTOs;
using WeChatReceiptBot.API.Models;

namespace WeChatReceiptBot.API.Services
{
    public interface IWeb3Service
    {
        // Smart contract deployment
/*        Task<SmartContractResponse> DeployBillContractAsync(GenerateBillContractRequest request);
        Task<SmartContractResponse> DeployPaymentContractAsync(GeneratePaymentContractRequest request);
        
        // Contract status queries
        Task<BillContractStatusResponse> GetBillContractStatusAsync(string contractAddress);
        Task<PaymentContractStatusResponse> GetPaymentContractStatusAsync(string contractAddress);
        
        // Wallet operations
        Task<Web3WalletConnectResponse> ValidateWalletSignatureAsync(Web3WalletConnectRequest request);*/
        Task<bool> IsValidWalletAddressAsync(string walletAddress);
        Task<decimal> GetWalletBalanceAsync(string walletAddress);
        
        // Transaction operations
        Task<string> SendTransactionAsync(string fromAddress, string toAddress, decimal amount, string privateKey);
        Task<bool> IsTransactionConfirmedAsync(string transactionHash);
        Task<decimal> GetTransactionGasFeeAsync(string transactionHash);
        
        // Contract interactions
        Task<bool> MarkBillAsSettledAsync(string contractAddress, string settlerAddress, string privateKey);
        Task<bool> PayBillShareAsync(string contractAddress, string payerAddress, decimal amount, string privateKey);
        Task<decimal> GetBillTotalAmountAsync(string contractAddress);
        Task<decimal> GetBillSettledAmountAsync(string contractAddress);
        Task<List<(string Address, decimal Amount)>> GetBillBeneficiariesAsync(string contractAddress);
        
        // Network operations
        Task<string> GetCurrentNetworkAsync();
        Task<bool> SwitchNetworkAsync(string networkName);
        Task<decimal> GetCurrentGasPriceAsync();
        Task<decimal> EstimateGasAsync(string contractAddress, string methodName, params object[] parameters);
        
        // Event monitoring
        Task<List<string>> GetContractEventsAsync(string contractAddress, string eventName, DateTime? fromDate = null);
        Task<bool> SubscribeToContractEventsAsync(string contractAddress, string eventName, Func<object, Task> eventHandler);
        
        // Utility functions
        Task<string> GenerateWalletAddressAsync();
        Task<string> GeneratePrivateKeyAsync();
        Task<string> GetAddressFromPrivateKeyAsync(string privateKey);
        Task<bool> ValidatePrivateKeyAsync(string privateKey);
        
        // Token operations (if using custom tokens)
        Task<decimal> GetTokenBalanceAsync(string walletAddress, string tokenContractAddress);
        Task<string> TransferTokenAsync(string fromAddress, string toAddress, decimal amount, string tokenContractAddress, string privateKey);
        
        // Multi-signature operations
        Task<string> CreateMultiSigWalletAsync(List<string> ownerAddresses, int requiredSignatures);
        Task<bool> SubmitMultiSigTransactionAsync(string multiSigAddress, string toAddress, decimal amount, string data, string privateKey);
        Task<bool> ConfirmMultiSigTransactionAsync(string multiSigAddress, int transactionId, string privateKey);
        
        // Service configuration
        Task<bool> UpdateWeb3ConfigAsync(string rpcUrl, string networkId, string chainId);
        Task<string> GetWeb3ConfigAsync();
        Task<bool> IsServiceConnectedAsync();
    }
}

