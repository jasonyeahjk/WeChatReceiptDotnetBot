namespace Web3Service.API.Application.DTOs.Web3DTOs;

public class NetworkStatusResponseDto
{
    public string NetworkName { get; set; } = string.Empty;
    public long BlockNumber { get; set; }
    public decimal GasPrice { get; set; }
    public bool IsSynchronizing { get; set; }
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

