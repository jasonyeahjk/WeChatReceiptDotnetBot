# Web3拼团记账微信助手项目文档

## 项目概述

Web3拼团记账微信助手是一个基于.NET 8和Vue 3架构的分布式记账应用，采用领域驱动设计（DDD）架构模式，集成了NebulaGraph图数据库、Apache Doris实时数据仓库和Donut图像识别技术。该系统允许用户通过微信进行拼团记账，支持单据识别、智能合约生成和分布式记账功能。

### 技术栈统一

项目已完成技术栈统一重构，所有后端服务均采用.NET 8和DDD架构：

- **WeChatReceiptBot.API**：主后端API服务
- **DonutReceiptService.API**：单据识别服务
- **DonutPaymentService.API**：支付记录识别服务  
- **Web3Service.API**：区块链和智能合约服务

### 核心功能

1. **微信交互**：用户可以在微信中@助手，发送记账指令和单据图片
2. **单据识别**：使用Donut模型自动识别收据和支付记录
3. **智能合约**：基于识别结果生成智能合约，确保记账透明和不可篡改
4. **分布式记账**：多用户可以参与同一账本的记账和查询
5. **用户画像**：使用图数据库存储和分析用户关系和行为
6. **实时数据分析**：使用Doris进行实时数据分析和报表生成

## 系统架构

系统采用微服务架构和领域驱动设计（DDD）模式，主要包含以下组件：

### 后端服务（.NET 8 DDD架构）

1. **WeChatReceiptBot.API**：主后端API服务
   - 用户认证和授权
   - 拼团管理
   - 账单和交易管理
   - 统一的错误处理和API响应

2. **DonutReceiptService.API**：单据识别服务
   - 收据图像识别
   - 商家信息提取
   - 商品明细解析
   - 识别历史管理

3. **DonutPaymentService.API**：支付记录识别服务
   - 支付凭证识别
   - 支付信息提取
   - 交易记录解析
   - 支付历史管理

4. **Web3Service.API**：区块链和智能合约服务
   - 区块链账户管理
   - 智能合约部署
   - 交易发送和查询
   - 合约方法调用

### 前端应用

5. **wechat-receipt-frontend**：Vue 3前端应用
   - 响应式用户界面
   - 拼团管理界面
   - 收据扫描功能
   - 实时数据展示

### 数据存储

6. **NebulaGraph数据库**：存储用户关系和行为图谱
7. **Apache Doris**：实时数据仓库，用于数据分析
8. **区块链网络**：智能合约和分布式账本

### DDD架构层次

每个.NET 8服务都采用标准的DDD分层架构：

```
├── Domain/                    # 领域层
│   ├── Entities/             # 实体
│   ├── ValueObjects/         # 值对象
│   ├── Repositories/         # 仓储接口
│   └── Services/             # 领域服务接口
├── Application/              # 应用层
│   ├── DTOs/                 # 数据传输对象
│   ├── Interfaces/           # 应用服务接口
│   └── Services/             # 应用服务实现
├── Infrastructure/           # 基础设施层
│   ├── Repositories/         # 仓储实现
│   └── Services/             # 外部服务集成
├── Controllers/              # 控制器层
└── Common/                   # 公共组件
    ├── Errors/               # 错误处理
    └── Extensions/           # 扩展方法
```

### 架构图

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           微信用户界面                                        │
└─────────────────────────┬───────────────────────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────────────────────┐
│                    Vue 3 前端应用                                            │
│                (wechat-receipt-frontend)                                    │
└─────────────────────────┬───────────────────────────────────────────────────┘
                          │ HTTP/REST API
┌─────────────────────────▼───────────────────────────────────────────────────┐
│                  .NET 8 主后端API服务                                        │
│                 (WeChatReceiptBot.API)                                      │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │   用户管理   │ │   拼团管理   │ │   账单管理   │ │   交易管理   │           │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘           │
└─────────────────┬───────────────┬───────────────┬───────────────────────────┘
                  │               │               │
        ┌─────────▼─────────┐ ┌───▼────────┐ ┌───▼──────────────┐
        │ DonutReceiptService│ │DonutPayment│ │   Web3Service    │
        │       .API         │ │Service.API │ │      .API        │
        │                    │ │            │ │                  │
        │ ┌─────────────────┐│ │┌──────────┐│ │┌────────────────┐│
        │ │   收据识别      ││ ││ 支付识别 ││ ││  区块链账户    ││
        │ │   商家提取      ││ ││ 交易解析 ││ ││  智能合约      ││
        │ │   明细解析      ││ ││ 历史管理 ││ ││  交易管理      ││
        │ └─────────────────┘│ │└──────────┘│ │└────────────────┘│
        └─────────┬─────────┘ └─────┬──────┘ └─────┬──────────────┘
                  │                 │              │
        ┌─────────▼─────────┐ ┌─────▼──────┐ ┌─────▼──────────────┐
        │   Donut AI模型    │ │  支付API   │ │    区块链网络      │
        │   (图像识别)      │ │  集成服务  │ │   (以太坊/BSC)     │
        └───────────────────┘ └────────────┘ └────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                            数据存储层                                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────────────────┐  │
│  │  NebulaGraph    │  │  Apache Doris   │  │      智能合约存储           │  │
│  │   图数据库      │  │  实时数据仓库   │  │   (区块链分布式账本)        │  │
│  │                 │  │                 │  │                             │  │
│  │ • 用户关系图谱  │  │ • 交易数据分析  │  │ • 账单智能合约              │  │
│  │ • 行为分析      │  │ • 实时报表      │  │ • 支付智能合约              │  │
│  │ • 社交网络      │  │ • 数据挖掘      │  │ • 不可篡改记录              │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 项目结构

```
WeChatReceiptBot/
├── DEPLOYMENT_GUIDE.md                    # 部署指南
├── project_documentation.md               # 项目文档
├── presentation/                          # 项目演示文稿
│   ├── images/                           # 演示图片
│   └── *.html                            # HTML幻灯片
├── smart_contracts/                       # 智能合约
│   ├── BillContract.sol                  # 账单合约
│   └── PaymentContract.sol               # 支付合约
├── wechat-receipt-frontend/              # Vue 3前端应用
│   ├── src/
│   │   ├── components/                   # Vue组件
│   │   ├── App.jsx                       # 主应用组件
│   │   └── App.css                       # 样式文件
│   ├── index.html                        # 入口HTML
│   └── package.json                      # 依赖配置
├── WeChatReceiptBot.API/                 # 主后端API服务 (.NET 8 DDD)
│   ├── Application/                      # 应用层
│   │   ├── DTOs/                         # 数据传输对象
│   │   ├── Interfaces/                   # 应用服务接口
│   │   └── Services/                     # 应用服务实现
│   ├── Common/                           # 公共组件
│   │   ├── Errors/                       # 错误处理
│   │   └── Extensions/                   # 扩展方法
│   ├── Controllers/                      # 控制器
│   ├── Domain/                           # 领域层
│   │   ├── Entities/                     # 实体
│   │   ├── ValueObjects/                 # 值对象
│   │   ├── Repositories/                 # 仓储接口
│   │   └── Services/                     # 领域服务接口
│   ├── Infrastructure/                   # 基础设施层
│   │   ├── Repositories/                 # 仓储实现
│   │   └── Services/                     # 外部服务集成
│   ├── Models/                           # 数据模型
│   ├── Services/                         # 业务服务
│   ├── Program.cs                        # 程序入口
│   ├── appsettings.json                  # 配置文件
│   └── WeChatReceiptBot.API.csproj       # 项目文件
├── DonutReceiptService.API/              # 单据识别服务 (.NET 8 DDD)
│   ├── Application/                      # 应用层
│   ├── Common/                           # 公共组件
│   ├── Controllers/                      # 控制器
│   ├── Domain/                           # 领域层
│   ├── Infrastructure/                   # 基础设施层
│   ├── Program.cs                        # 程序入口
│   └── DonutReceiptService.API.csproj    # 项目文件
├── DonutPaymentService.API/              # 支付记录识别服务 (.NET 8 DDD)
│   ├── Application/                      # 应用层
│   ├── Common/                           # 公共组件
│   ├── Controllers/                      # 控制器
│   ├── Domain/                           # 领域层
│   ├── Infrastructure/                   # 基础设施层
│   ├── Program.cs                        # 程序入口
│   └── DonutPaymentService.API.csproj    # 项目文件
├── Web3Service.API/                      # 区块链服务 (.NET 8 DDD)
│   ├── Application/                      # 应用层
│   ├── Common/                           # 公共组件
│   ├── Controllers/                      # 控制器
│   ├── Domain/                           # 领域层
│   ├── Infrastructure/                   # 基础设施层
│   ├── Program.cs                        # 程序入口
│   └── Web3Service.API.csproj            # 项目文件
├── donut-receipt-service/                # 原始Python服务（已重构）
│   └── src/
├── donut-payment-service/                # 原始Python服务（已重构）
│   └── src/
└── web3-service/                         # 原始Python服务（已重构）
    └── src/
```

## 技术栈

### 后端（.NET 8 DDD架构）
- **.NET 8**：核心API框架
- **ASP.NET Core Web API**：RESTful API开发
- **FluentResults**：函数式错误处理
- **Entity Framework Core**：ORM框架（可选）
- **ASP.NET Core Identity**：用户认证
- **Swagger/OpenAPI**：API文档生成
- **Problem Details**：标准化错误响应
- **Nethereum**：以太坊区块链交互
- **NebulaGraph Client**：图数据库交互
- **Apache Doris Client**：数据仓库交互

### 前端
- **Vue 3**：前端框架
- **React**：组件化开发（部分组件）
- **Tailwind CSS**：样式框架
- **Axios**：HTTP客户端
- **Web3.js**：区块链交互

### 图像识别（已重构为.NET 8）
- **Donut模型集成**：文档理解
- **ASP.NET Core**：识别服务API
- **图像处理库**：图像预处理

### 区块链（已重构为.NET 8）
- **Solidity**：智能合约开发
- **Nethereum**：.NET区块链交互库
- **Ethereum/BSC**：区块链平台

### 开发工具和部署
- **Docker**：容器化部署
- **Git**：版本控制
- **Visual Studio/VS Code**：开发环境
- **Postman**：API测试

## 数据库设计

### NebulaGraph图数据库

#### 顶点（Vertices）
1. **User**：用户节点
   - 属性：userId, username, nickname, walletAddress, registrationDate

2. **Group**：拼团节点
   - 属性：groupId, groupName, description, createdAt, currency

3. **Bill**：账单节点
   - 属性：billId, billName, amount, currency, createdAt, status

4. **Transaction**：交易节点
   - 属性：transactionId, amount, description, transactionType, timestamp

#### 边（Edges）
1. **BELONGS_TO**：用户属于拼团
   - 属性：role, joinedAt

2. **CREATED**：用户创建拼团/账单/交易
   - 属性：createdAt

3. **PART_OF**：账单属于拼团
   - 属性：addedAt

4. **PAID**：用户支付交易
   - 属性：paidAt, paymentMethod

5. **RECEIVED**：用户接收付款
   - 属性：receivedAt

6. **FRIENDS_WITH**：用户之间的朋友关系
   - 属性：since, strength

### Apache Doris数据仓库

#### 事实表
1. **fact_transactions**：交易事实表
   - 列：transaction_id, bill_id, user_id, amount, transaction_type, timestamp, payment_method, status

2. **fact_bills**：账单事实表
   - 列：bill_id, group_id, creator_id, total_amount, settled_amount, created_at, settled_at, status

3. **fact_receipts**：收据事实表
   - 列：receipt_id, transaction_id, image_path, recognition_confidence, recognized_at, merchant, items_json

#### 维度表
1. **dim_users**：用户维度表
   - 列：user_id, username, nickname, registration_date, last_active_date, wallet_address

2. **dim_groups**：拼团维度表
   - 列：group_id, group_name, description, created_at, creator_id, member_count, currency

3. **dim_time**：时间维度表
   - 列：date_id, date, day, month, quarter, year, is_weekend, is_holiday

## API接口设计

### 认证API
- `POST /api/auth/register`：用户注册
- `POST /api/auth/login`：用户登录
- `POST /api/auth/refresh-token`：刷新令牌
- `POST /api/auth/bind-wallet`：绑定钱包地址

### 用户API
- `GET /api/users/profile`：获取用户资料
- `PUT /api/users/profile`：更新用户资料
- `GET /api/users/{id}/activities`：获取用户活动
- `GET /api/users/{id}/statistics`：获取用户统计数据

### 拼团API
- `POST /api/groups`：创建拼团
- `GET /api/groups`：获取拼团列表
- `GET /api/groups/{id}`：获取拼团详情
- `PUT /api/groups/{id}`：更新拼团信息
- `POST /api/groups/{id}/members`：添加拼团成员
- `DELETE /api/groups/{id}/members/{userId}`：移除拼团成员

### 账单API
- `POST /api/bills`：创建账单
- `GET /api/bills`：获取账单列表
- `GET /api/bills/{id}`：获取账单详情
- `PUT /api/bills/{id}`：更新账单信息
- `POST /api/bills/{id}/settle`：结算账单

### 交易API
- `POST /api/transactions`：创建交易
- `GET /api/transactions`：获取交易列表
- `GET /api/transactions/{id}`：获取交易详情
- `PUT /api/transactions/{id}`：更新交易信息
- `POST /api/transactions/{id}/verify`：验证交易

### 单据识别API
- `POST /api/recognize/receipt`：识别收据
- `POST /api/recognize/payment`：识别支付记录
- `POST /api/recognize/document`：识别通用文档

### Web3 API
- `POST /api/web3/account/create`：创建区块链账户
- `GET /api/web3/account/balance/{address}`：获取账户余额
- `POST /api/web3/bill/create`：创建账单智能合约
- `POST /api/web3/transaction/add`：添加交易到智能合约
- `POST /api/web3/payment/record`：记录支付到区块链

## 智能合约设计

### BillContract.sol
管理拼团账单和费用分摊的智能合约。

#### 数据结构
- **Bill**：账单结构体
  - billId, billName, description, creator, totalAmount, settledAmount, currency, isSettled, createdAt, members, memberShares, memberPaid

- **Transaction**：交易结构体
  - transactionId, billId, payer, amount, description, transactionType, timestamp, isSettled, beneficiaries, beneficiaryAmounts

#### 主要功能
- `createBill`：创建新账单
- `addMemberToBill`：添加成员到账单
- `addTransaction`：添加交易到账单
- `makePayment`：进行支付结算
- `settleTransaction`：结算交易
- `getBill`：获取账单信息
- `getMemberBillInfo`：获取成员账单信息
- `getTransaction`：获取交易信息

### PaymentContract.sol
管理支付记录和验证的智能合约。

#### 数据结构
- **PaymentRecord**：支付记录结构体
  - paymentId, transactionId, payer, receiver, amount, currency, paymentMethod, paymentDate, createdAt, status, notes, imageHash, isVerified, verifiedBy, verifiedAt

#### 主要功能
- `recordPayment`：记录新支付
- `verifyPayment`：验证支付记录
- `updatePaymentStatus`：更新支付状态
- `getPaymentRecord`：获取支付记录
- `getUserPayments`：获取用户支付记录
- `getUserReceivedPayments`：获取用户收款记录
- `getUserTotalPaid`：获取用户总支付金额
- `getUserTotalReceived`：获取用户总收款金额

## 前端界面设计

### 主要页面

1. **登录/注册页面**
   - 用户登录和注册表单
   - 钱包连接选项

2. **仪表板页面**
   - 用户账单概览
   - 最近交易记录
   - 快速操作按钮

3. **拼团管理页面**
   - 创建新拼团
   - 拼团列表和详情
   - 成员管理

4. **账单管理页面**
   - 创建新账单
   - 账单列表和详情
   - 交易记录

5. **单据扫描页面**
   - 相机拍照/上传图片
   - 识别结果预览
   - 确认和编辑选项

6. **支付记录页面**
   - 支付记录列表
   - 支付详情和状态
   - 验证支付选项

7. **用户资料页面**
   - 个人信息设置
   - 钱包管理
   - 通知设置

### 组件设计

1. **Header组件**
   - 导航菜单
   - 用户信息和头像
   - 通知图标

2. **GroupManagement组件**
   - 拼团列表
   - 拼团创建表单
   - 成员管理界面

3. **ReceiptScanner组件**
   - 相机/上传界面
   - 识别结果展示
   - 编辑和确认按钮

4. **Dashboard组件**
   - 统计卡片
   - 最近活动列表
   - 快速操作按钮

5. **Footer组件**
   - 版权信息
   - 链接和社交媒体
   - 联系方式

## 系统流程

### 单据识别流程

1. 用户在微信中@助手，发送"记账"和单据图片
2. 系统接收图片并传递给Donut识别服务
3. Donut服务识别单据内容，提取关键信息
4. 系统根据识别结果创建交易记录
5. 生成智能合约并记录到区块链
6. 返回确认信息给用户

### 支付记录流程

1. 用户在微信中@助手，发送"销账"和支付记录图片
2. 系统接收图片并传递给Donut识别服务
3. Donut服务识别支付记录内容
4. 系统验证支付记录与交易的匹配
5. 更新智能合约中的支付状态
6. 返回确认信息给用户

### 拼团记账流程

1. 用户创建拼团并邀请成员
2. 成员加入拼团
3. 任何成员可以添加账单和交易
4. 系统自动计算每个成员的应付金额
5. 成员进行支付并上传支付凭证
6. 系统验证支付并更新结算状态
7. 当所有交易结算完成，账单标记为已结清

## 安全考虑

### 数据安全
- 使用HTTPS加密通信
- 实现JWT令牌认证
- 加密敏感数据
- 定期备份数据库

### 智能合约安全
- 合约审计
- 权限控制
- 防重入攻击
- Gas优化

### 隐私保护
- 用户数据匿名化
- 符合GDPR要求
- 数据访问控制
- 用户同意机制

## 性能优化

### 数据库优化
- NebulaGraph索引优化
- Doris查询优化
- 数据分区策略
- 缓存机制

### API优化
- 响应缓存
- 异步处理
- 批量操作
- 分页和限流

### 前端优化
- 懒加载
- 组件缓存
- 资源压缩
- CDN加速

## 测试策略

### 单元测试
- 使用xUnit测试.NET组件
- 使用Jest测试Vue组件
- 使用Pytest测试Python服务

### 集成测试
- API端点测试
- 数据库交互测试
- 服务间通信测试

### 端到端测试
- 用户流程测试
- 性能和负载测试
- 安全性测试

## 部署架构

### 开发环境
- 本地开发服务器
- Docker容器化服务
- 测试数据库

### 测试环境
- 云服务器部署
- CI/CD流水线
- 自动化测试

### 生产环境
- 高可用集群
- 负载均衡
- 自动扩展
- 监控和告警

## 未来扩展

### 功能扩展
- 多链支持
- 高级数据分析
- AI辅助记账
- 移动应用开发

### 技术升级
- 升级到更新版本的框架
- 优化数据库性能
- 增强AI模型精度
- 扩展区块链功能

## 常见问题解答

### 技术问题
1. **如何处理区块链交易延迟？**
   - 实现异步确认机制
   - 提供交易状态跟踪
   - 使用事件监听更新状态

2. **如何提高单据识别准确率？**
   - 使用预处理增强图像质量
   - 实现用户反馈机制
   - 定期重新训练模型

3. **如何处理高并发请求？**
   - 实现缓存机制
   - 使用负载均衡
   - 优化数据库查询

### 用户问题
1. **如何开始使用系统？**
   - 注册账户
   - 创建或加入拼团
   - 扫描收据开始记账

2. **如何解决账单分摊争议？**
   - 使用区块链记录确保透明
   - 提供详细的交易历史
   - 实现投票机制解决争议

3. **如何保护我的数据安全？**
   - 所有数据加密存储
   - 区块链确保数据不可篡改
   - 严格的访问控制机制

## 项目团队

### 开发团队
- 项目经理：负责整体项目管理和协调
- 后端开发：负责.NET API和数据库开发
- 前端开发：负责Vue界面开发
- 区块链开发：负责智能合约和Web3集成
- AI工程师：负责Donut模型部署和优化

### 支持团队
- 产品经理：负责产品规划和需求分析
- UI/UX设计师：负责界面设计和用户体验
- 测试工程师：负责质量保证和测试
- 运维工程师：负责部署和维护

## 项目时间线

### 第一阶段：基础开发（1-2个月）
- 技术栈研究和架构设计
- 系统架构和数据库设计
- 后端API服务开发
- Donut图像识别服务部署

### 第二阶段：核心功能（2-3个月）
- Vue3前端界面开发
- 智能合约开发和Web3集成
- 系统集成测试和部署
- 项目文档和演示

### 第三阶段：优化和扩展（持续）
- 性能优化
- 功能扩展
- 用户反馈收集和改进
- 安全审计和更新

## 联系与支持

### 技术支持
- 邮箱：support@wechatreceiptbot.com
- 文档：https://docs.wechatreceiptbot.com
- GitHub：https://github.com/wechatreceiptbot

### 贡献指南
- 代码贡献流程
- 问题报告指南
- 功能请求流程

### 许可证
本项目采用MIT许可证。详情请参阅LICENSE文件。

