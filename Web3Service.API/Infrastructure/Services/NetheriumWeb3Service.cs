using FluentResults;
using Microsoft.Extensions.Logging;
using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;
using NBitcoin;
using Nethereum.Web3;
using Web3Service.API.Common.Errors;
using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.Services;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Infrastructure.Services;

/// <summary>
/// 基于Nethereum的Web3区块链服务实现
/// </summary>
public class NetheriumWeb3Service : IWeb3BlockchainService
{
    private readonly ILogger<NetheriumWeb3Service> _logger;
    private readonly Web3 _web3;
    private readonly string _rpcUrl;

    public NetheriumWeb3Service(ILogger<NetheriumWeb3Service> logger, IConfiguration configuration)
    {
        _logger = logger;
        _rpcUrl = configuration.GetConnectionString("EthereumRpc") ?? "https://mainnet.infura.io/v3/YOUR_PROJECT_ID";
        _web3 = new Web3(_rpcUrl);
    }

    public async Task<Result<BlockchainAccount>> CreateAccountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new blockchain account");

            // 生成新的助记词和账户
            var mnemonic = new Mnemonic(NBitcoin.Wordlist.English, NBitcoin.WordCount.Twelve);
            var wallet = new Wallet(mnemonic.ToString(), "");
            var account = wallet.GetAccount(0);

            var blockchainAccount = new BlockchainAccount(
                account.Address,
                account.PrivateKey,
                account.PublicKey,
                AccountType.ExternallyOwnedAccount);

            _logger.LogInformation("Successfully created account with address: {Address}", account.Address);
            return Result.Ok(blockchainAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blockchain account");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_CREATION_FAILED, "创建账户失败", ex));
        }
    }

    public async Task<Result<BlockchainAccount>> ImportAccountFromPrivateKeyAsync(string privateKey, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Importing account from private key");

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_INVALID_PRIVATE_KEY, "私钥不能为空"));
            }

            // 验证私钥格式
            if (!privateKey.StartsWith("0x"))
            {
                privateKey = "0x" + privateKey;
            }

            var account = new Account(privateKey);
            
            var blockchainAccount = new BlockchainAccount(
                account.Address,
                account.PrivateKey,
                account.PublicKey,
                AccountType.ExternallyOwnedAccount);

            _logger.LogInformation("Successfully imported account with address: {Address}", account.Address);
            return Result.Ok(blockchainAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing account from private key");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_IMPORT_FAILED, "导入账户失败", ex));
        }
    }

    public async Task<Result<decimal>> GetBalanceAsync(string address, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting balance for address: {Address}", address);

            if (!BlockchainAccount.IsValidAddress(address))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_INVALID_ADDRESS, "无效的地址格式"));
            }

            var balanceWei = await _web3.Eth.GetBalance.SendRequestAsync(address);
            var balanceEth = Web3.Convert.FromWei(balanceWei);

            _logger.LogInformation("Balance for {Address}: {Balance} ETH", address, balanceEth);
            return Result.Ok(balanceEth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting balance for address: {Address}", address);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_BALANCE_QUERY_FAILED, "查询余额失败", ex));
        }
    }

    public async Task<Result<string>> SendTransactionAsync(
        string fromAddress,
        string toAddress,
        decimal amount,
        string? privateKey = null,
        decimal? gasPrice = null,
        long? gasLimit = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending transaction from {FromAddress} to {ToAddress}, amount: {Amount} ETH", 
                fromAddress, toAddress, amount);

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_VALIDATION_MISSING_PRIVATE_KEY, "缺少私钥"));
            }

            if (!BlockchainAccount.IsValidAddress(fromAddress) || !BlockchainAccount.IsValidAddress(toAddress))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_INVALID_ADDRESS, "无效的地址格式"));
            }

            if (amount <= 0)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_VALIDATION_INVALID_AMOUNT, "转账金额必须大于0"));
            }

            var account = new Account(privateKey);
            var web3 = new Web3(account, _rpcUrl);

            // 获取当前Gas价格（如果未指定）
            if (!gasPrice.HasValue)
            {
                var currentGasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                gasPrice = Web3.Convert.FromWei(currentGasPrice);
            }

            // 估算Gas限制（如果未指定）
            if (!gasLimit.HasValue)
            {
                var estimatedGas = await web3.Eth.GetEtherTransferService()
                    .EstimateGasAsync(toAddress, amount);
                gasLimit = (long)estimatedGas;
            }

            // 发送交易
            var transactionHash = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(toAddress, amount, gasPrice, gasLimit);

            _logger.LogInformation("Transaction sent successfully. Hash: {TransactionHash}", transactionHash);
            return Result.Ok(transactionHash.TransactionHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transaction");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_SEND_FAILED, "发送交易失败", ex));
        }
    }

    public async Task<Result<TransactionRecord>> GetTransactionAsync(string transactionHash, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting transaction details for hash: {TransactionHash}", transactionHash);

            if (!TransactionRecord.IsValidTransactionHash(transactionHash))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_INVALID_HASH, "无效的交易哈希"));
            }

            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            if (transaction == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_NOT_FOUND, "交易不存在"));
            }

            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            var transactionRecord = new TransactionRecord(
                transactionHash,
                transaction.From,
                transaction.To ?? string.Empty,
                Web3.Convert.FromWei(transaction.Value),
                Web3.Convert.FromWei(transaction.GasPrice),
                (long)transaction.Gas.Value,
                Guid.Empty); // 需要从外部提供AccountId

            if (receipt != null)
            {
                var status = receipt.Status.Value == 1 ? TransactionStatus.Confirmed : TransactionStatus.Failed;
                if (status == TransactionStatus.Confirmed)
                {
                    transactionRecord.MarkAsConfirmed(
                        (long)receipt.BlockNumber.Value,
                        receipt.BlockHash,
                        (int)receipt.TransactionIndex.Value,
                        (long)receipt.GasUsed.Value,
                        receipt.ContractAddress);
                }
                else
                {
                    transactionRecord.MarkAsFailed("交易执行失败");
                }
            }

            return Result.Ok(transactionRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction: {TransactionHash}", transactionHash);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED, "查询交易失败", ex));
        }
    }

    public async Task<Result<object>> GetTransactionReceiptAsync(string transactionHash, CancellationToken cancellationToken = default)
    {
        try
        {
            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            if (receipt == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_NOT_FOUND, "交易收据不存在"));
            }

            return Result.Ok<object>(receipt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction receipt: {TransactionHash}", transactionHash);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED, "查询交易收据失败", ex));
        }
    }

    public async Task<Result<long>> EstimateGasAsync(
        string fromAddress,
        string toAddress,
        decimal amount,
        string? data = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var callInput = new Nethereum.RPC.Eth.DTOs.CallInput
            {
                From = fromAddress,
                To = toAddress,
                Value = new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(amount)),
                Data = data
            };

            var estimatedGas = await _web3.Eth.Transactions.EstimateGas.SendRequestAsync(callInput);
            return Result.Ok((long)estimatedGas.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating gas");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_GAS_ESTIMATION_FAILED, "Gas估算失败", ex));
        }
    }

    public async Task<Result<decimal>> GetGasPriceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
            return Result.Ok(Web3.Convert.FromWei(gasPrice.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gas price");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_GAS_PRICE_QUERY_FAILED, "查询Gas价格失败", ex));
        }
    }

    public async Task<Result<long>> GetLatestBlockNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var blockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return Result.Ok((long)blockNumber.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest block number");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_BLOCK_QUERY_FAILED, "查询最新区块号失败", ex));
        }
    }

    public async Task<Result<object>> GetBlockAsync(long blockNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(
                new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber));
            
            if (block == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_BLOCK_QUERY_FAILED, "区块不存在"));
            }

            return Result.Ok<object>(block);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting block: {BlockNumber}", blockNumber);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_BLOCK_QUERY_FAILED, "查询区块失败", ex));
        }
    }

    public async Task<Result<int>> GetNetworkIdAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var networkId = await _web3.Net.Version.SendRequestAsync();
            return Result.Ok(int.Parse(networkId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting network ID");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_NETWORK_CONNECTION_FAILED, "获取网络ID失败", ex));
        }
    }

    public async Task<Result<bool>> IsConnectedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var isListening = await _web3.Net.Listening.SendRequestAsync();
            return Result.Ok(isListening);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking network connection");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_NETWORK_CONNECTION_FAILED, "检查网络连接失败", ex));
        }
    }
}

