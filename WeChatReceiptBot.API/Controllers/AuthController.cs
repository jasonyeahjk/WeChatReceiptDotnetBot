using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentResults;
using WeChatReceiptBot.API.DTOs;
using WeChatReceiptBot.API.Services;
using WeChatReceiptBot.API.Common;
using WeChatReceiptBot.API.Extensions;
using System.Security.Claims;

namespace WeChatReceiptBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserProfileResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("User registration attempt for username: {Username}", request.Username);

                // 验证请求数据
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return this.ValidationError("Username", "用户名不能为空");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return this.ValidationError("Password", "密码不能为空");
                }

                // TODO: 实现用户注册逻辑
                // 1. 验证用户名是否已存在
                // 2. 创建新用户
                // 3. 生成JWT令牌
                
                var userResponse = new UserProfileResponse
                {
                    UserId = Guid.NewGuid(),
                    Username = request.Username,
                    Nickname = request.Nickname,
                    WalletAddress = null,
                    CreatedAt = DateTime.UtcNow
                };

                return this.Created($"/api/users/{userResponse.UserId.ToString()}", userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        [HttpPost("login")]
       public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request){
            try
            {
                _logger.LogInformation("User login attempt for username: {Username}", request.Username);

                // 验证请求数据
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return this.ValidationError("Username", "用户名不能为空");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return this.ValidationError("Password", "密码不能为空");
                }

                // TODO: 实现用户登录逻辑
                // 1. 验证用户凭据
                // 2. 生成JWT令牌
                // 3. 返回用户信息和令牌

                // 模拟用户验证失败
                if (request.Username == "invalid_user")
                {
                    return this.Error(ErrorCodes.AUTH_INVALID_CREDENTIALS, 401);
                }

                var loginResponse = new AuthResponse
                {
                    Token = "sample_jwt_token_here",
                    RefreshToken = "sample_refresh_token_here",
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = new UserProfileResponse
                    {
                        UserId = Guid.NewGuid(),
                        Username = request.Username,
                        Nickname = "用户昵称",
                        WalletAddress = null,
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    }
                };

                return this.Success(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                _logger.LogInformation("Token refresh attempt");

                // 验证请求数据
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return this.ValidationError("RefreshToken", "刷新令牌不能为空");
                }

                // TODO: 实现令牌刷新逻辑
                // 1. 验证刷新令牌
                // 2. 生成新的访问令牌
                // 3. 返回新令牌

                // 模拟无效刷新令牌
                if (request.RefreshToken == "invalid_token")
                {
                    return this.Error(ErrorCodes.AUTH_INVALID_TOKEN, 401);
                }

                var refreshResponse = new RefreshTokenResponse
                {
                    Token = "new_jwt_token_here",
                    RefreshToken = "new_refresh_token_here",
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                return this.Success(refreshResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 绑定钱包地址
        /// </summary>
        [HttpPost("bind-wallet")]
        [Authorize]
        public async Task<ActionResult> BindWallet([FromBody] BindWalletRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Wallet binding attempt for user: {UserId}", userId);

                // 验证请求数据
                if (string.IsNullOrWhiteSpace(request.WalletAddress))
                {
                    return this.ValidationError("WalletAddress", "钱包地址不能为空");
                }

                // 验证钱包地址格式
                if (!IsValidWalletAddress(request.WalletAddress))
                {
                    return this.Error(ErrorCodes.AUTH_INVALID_WALLET_ADDRESS, 400);
                }

                // TODO: 实现钱包绑定逻辑
                // 1. 验证钱包地址格式
                // 2. 检查钱包地址是否已被使用
                // 3. 更新用户钱包地址

                // 模拟钱包已绑定
                if (request.WalletAddress == "0x1234567890abcdef")
                {
                    return this.Error(ErrorCodes.AUTH_WALLET_ALREADY_BOUND, 409);
                }

                return this.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during wallet binding");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserProfileResponse>> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Profile request for user: {UserId}", userId);

                if (string.IsNullOrEmpty(userId))
                {
                    return this.Error(ErrorCodes.AUTH_INVALID_TOKEN, 401);
                }

                // TODO: 从数据库获取用户信息
                var userProfile = new UserProfileResponse
                {
                    UserId = Guid.Parse(userId!),
                    Username = "current_user",
                    Nickname = "当前用户",
                    WalletAddress = "0x1234567890abcdef",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                };

                return this.Success(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User logout for user: {UserId}", userId);

                // TODO: 实现登出逻辑
                // 1. 将令牌加入黑名单
                // 2. 清理用户会话

                return this.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 验证钱包地址格式
        /// </summary>
        private static bool IsValidWalletAddress(string address)
        {
            // 简单的以太坊地址格式验证
            return address.StartsWith("0x") && address.Length == 42;
        }
    }
}

