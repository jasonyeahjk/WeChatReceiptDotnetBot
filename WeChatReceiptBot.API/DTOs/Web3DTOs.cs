using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.DTOs.Web3DTOs
{
    public class NetworkStatusResponseDto
    {
        public bool IsConnected { get; set; }
        public long LatestBlockNumber { get; set; }
        public decimal GasPrice { get; set; }
        public string NetworkId { get; set; } = string.Empty;
    }

    public class Web3StatisticsDto
    {
        public int TotalTransactions { get; set; }
        public int SuccessfulTransactions { get; set; }
        public int FailedTransactions { get; set; }
        public long TotalGasUsed { get; set; }
        public string TotalGasCost { get; set; } = string.Empty;
        public int ContractsDeployed { get; set; }
        public int ContractCalls { get; set; }
        public string AverageGasPrice { get; set; } = string.Empty;
    }
}


