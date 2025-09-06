# Web3拼团记账微信助手 - 详细部署指南

## 项目概述

Web3拼团记账微信助手是一个基于.NET 8和Vue 3架构的分布式记账应用，采用领域驱动设计（DDD）架构模式，集成了NebulaGraph图数据库、Apache Doris实时数据仓库、Donut图像识别技术和智能合约功能。

### 技术栈统一重构

项目已完成技术栈统一重构，所有后端服务均采用.NET 8和DDD架构：

- **WeChatReceiptBot.API**：主后端API服务
- **DonutReceiptService.API**：单据识别服务（已重构为.NET 8）
- **DonutPaymentService.API**：支付记录识别服务（已重构为.NET 8）
- **Web3Service.API**：区块链和智能合约服务（已重构为.NET 8）

## 系统要求

### 硬件要求
- CPU: 4核心以上
- 内存: 8GB以上
- 存储: 100GB以上可用空间
- 网络: 稳定的互联网连接

### 软件要求
- 操作系统: Ubuntu 20.04+ / CentOS 8+ / Windows Server 2019+
- .NET 8 SDK
- Node.js 18.0+
- Python 3.8+
- Docker 20.10+
- Nginx 1.18+

## 项目结构

```
WeChatReceiptBot/
├── WeChatReceiptBot.API/              # .NET 8 主后端API服务 (DDD架构)
│   ├── Application/                   # 应用层
│   ├── Common/                        # 公共组件
│   ├── Controllers/                   # 控制器
│   ├── Domain/                        # 领域层
│   ├── Infrastructure/                # 基础设施层
│   └── Program.cs                     # 程序入口
├── DonutReceiptService.API/           # .NET 8 单据识别服务 (DDD架构)
│   ├── Application/                   # 应用层
│   ├── Common/                        # 公共组件
│   ├── Controllers/                   # 控制器
│   ├── Domain/                        # 领域层
│   └── Infrastructure/                # 基础设施层
├── DonutPaymentService.API/           # .NET 8 支付记录识别服务 (DDD架构)
│   ├── Application/                   # 应用层
│   ├── Common/                        # 公共组件
│   ├── Controllers/                   # 控制器
│   ├── Domain/                        # 领域层
│   └── Infrastructure/                # 基础设施层
├── Web3Service.API/                   # .NET 8 区块链服务 (DDD架构)
│   ├── Application/                   # 应用层
│   ├── Common/                        # 公共组件
│   ├── Controllers/                   # 控制器
│   ├── Domain/                        # 领域层
│   └── Infrastructure/                # 基础设施层
├── wechat-receipt-frontend/           # Vue 3 前端应用
│   ├── src/components/                # Vue组件
│   ├── src/App.jsx                    # 主应用组件
│   └── package.json                   # 依赖配置
├── smart_contracts/                   # 智能合约代码
│   ├── BillContract.sol               # 账单合约
│   └── PaymentContract.sol            # 支付合约
├── presentation/                      # 项目演示文稿
├── donut-receipt-service/             # 原始Python服务（已重构）
├── donut-payment-service/             # 原始Python服务（已重构）
├── web3-service/                      # 原始Python服务（已重构）
├── project_documentation.md           # 项目文档
└── DEPLOYMENT_GUIDE.md               # 部署指南（本文档）
```

## 部署步骤

### 第一步：环境准备

#### 1.1 安装基础软件

**Ubuntu/Debian系统：**
```bash
# 更新系统包
sudo apt update && sudo apt upgrade -y

# 安装基础工具
sudo apt install -y curl wget git unzip

# 安装.NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-8.0

# 安装Node.js 18
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# 安装Python 3.8+
sudo apt install -y python3 python3-pip python3-venv

# 安装Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# 安装Nginx
sudo apt install -y nginx
```

**CentOS/RHEL系统：**
```bash
# 更新系统包
sudo yum update -y

# 安装基础工具
sudo yum install -y curl wget git unzip

# 安装.NET 8 SDK
sudo rpm -Uvh https://packages.microsoft.com/config/centos/8/packages-microsoft-prod.rpm
sudo yum install -y dotnet-sdk-8.0

# 安装Node.js 18
curl -fsSL https://rpm.nodesource.com/setup_18.x | sudo bash -
sudo yum install -y nodejs

# 安装Python 3.8+
sudo yum install -y python3 python3-pip

# 安装Docker
sudo yum install -y docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker $USER

# 安装Nginx
sudo yum install -y nginx
```

#### 1.2 安装数据库

**NebulaGraph安装：**
```bash
# 下载NebulaGraph
wget https://github.com/vesoft-inc/nebula/releases/download/v3.6.0/nebula-graph-3.6.0.el8.x86_64.rpm

# 安装NebulaGraph
sudo rpm -ivh nebula-graph-3.6.0.el8.x86_64.rpm

# 启动NebulaGraph服务
sudo systemctl start nebula-graphd
sudo systemctl start nebula-metad
sudo systemctl start nebula-storaged

# 设置开机自启
sudo systemctl enable nebula-graphd
sudo systemctl enable nebula-metad
sudo systemctl enable nebula-storaged
```

**Apache Doris安装：**
```bash
# 下载Apache Doris
wget https://archive.apache.org/dist/doris/1.2/1.2.7/apache-doris-1.2.7-bin-x64.tar.gz

# 解压安装
tar -xzf apache-doris-1.2.7-bin-x64.tar.gz
sudo mv apache-doris-1.2.7-bin-x64 /opt/doris

# 配置环境变量
echo 'export DORIS_HOME=/opt/doris' >> ~/.bashrc
echo 'export PATH=$PATH:$DORIS_HOME/bin' >> ~/.bashrc
source ~/.bashrc

# 启动Doris服务
cd /opt/doris
./bin/start_fe.sh --daemon
./bin/start_be.sh --daemon
```

### 第二步：代码部署

#### 2.1 克隆项目代码
```bash
# 克隆项目（假设代码已上传到Git仓库）
git clone https://github.com/your-org/WeChatReceiptBot.git
cd WeChatReceiptBot
```

#### 2.2 部署.NET 8后端服务

**2.2.1 部署主后端API服务**

```bash
# 进入后端项目目录
cd WeChatReceiptBot.API

# 恢复NuGet包
dotnet restore

# 配置数据库连接字符串
cp appsettings.json appsettings.Production.json

# 编辑配置文件
nano appsettings.Production.json
```

**配置示例（appsettings.Production.json）：**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=wechatreceiptbot;Username=postgres;Password=your_password",
    "NebulaGraph": "127.0.0.1:9669",
    "Doris": "127.0.0.1:9030",
    "EthereumRpc": "https://mainnet.infura.io/v3/YOUR_PROJECT_ID"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-must-be-at-least-32-characters",
    "Issuer": "WeChatReceiptBot",
    "Audience": "WeChatReceiptBot-Users",
    "ExpirationHours": 24
  },
  "Blockchain": {
    "NetworkUrl": "http://localhost:8545",
    "ChainId": 1337,
    "BillContractAddress": "0x...",
    "PaymentContractAddress": "0x..."
  },
  "Services": {
    "DonutReceiptServiceUrl": "http://localhost:5001",
    "DonutPaymentServiceUrl": "http://localhost:5002",
    "Web3ServiceUrl": "http://localhost:5003"
  }
}
```

```bash
# 构建项目
dotnet build --configuration Release

# 发布项目
dotnet publish --configuration Release --output /var/www/wechatreceiptbot-api

# 创建systemd服务文件
sudo nano /etc/systemd/system/wechatreceiptbot-api.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=WeChat Receipt Bot API
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/wechatreceiptbot-api/WeChatReceiptBot.API.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=wechatreceiptbot-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

```bash
# 启动API服务
sudo systemctl daemon-reload
sudo systemctl enable wechatreceiptbot-api
sudo systemctl start wechatreceiptbot-api

# 检查服务状态
sudo systemctl status wechatreceiptbot-api
```

**2.2.2 部署Donut单据识别服务**

```bash
# 进入单据识别服务目录
cd ../DonutReceiptService.API

# 恢复NuGet包
dotnet restore

# 构建项目
dotnet build --configuration Release

# 发布项目
dotnet publish --configuration Release --output /var/www/donut-receipt-service

# 创建systemd服务文件
sudo nano /etc/systemd/system/donut-receipt-service.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=Donut Receipt Recognition Service
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/donut-receipt-service/DonutReceiptService.API.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=donut-receipt-service
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5001

[Install]
WantedBy=multi-user.target
```

```bash
# 启动服务
sudo systemctl daemon-reload
sudo systemctl enable donut-receipt-service
sudo systemctl start donut-receipt-service

# 检查服务状态
sudo systemctl status donut-receipt-service
```

**2.2.3 部署Donut支付记录识别服务**

```bash
# 进入支付记录识别服务目录
cd ../DonutPaymentService.API

# 恢复NuGet包
dotnet restore

# 构建项目
dotnet build --configuration Release

# 发布项目
dotnet publish --configuration Release --output /var/www/donut-payment-service

# 创建systemd服务文件
sudo nano /etc/systemd/system/donut-payment-service.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=Donut Payment Recognition Service
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/donut-payment-service/DonutPaymentService.API.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=donut-payment-service
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5002

[Install]
WantedBy=multi-user.target
```

```bash
# 启动服务
sudo systemctl daemon-reload
sudo systemctl enable donut-payment-service
sudo systemctl start donut-payment-service

# 检查服务状态
sudo systemctl status donut-payment-service
```

**2.2.4 部署Web3区块链服务**

```bash
# 进入Web3服务目录
cd ../Web3Service.API

# 恢复NuGet包
dotnet restore

# 构建项目
dotnet build --configuration Release

# 发布项目
dotnet publish --configuration Release --output /var/www/web3-service

# 创建systemd服务文件
sudo nano /etc/systemd/system/web3-service.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=Web3 Blockchain Service
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/web3-service/Web3Service.API.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=web3-service
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5003

[Install]
WantedBy=multi-user.target
```

```bash
# 启动服务
sudo systemctl daemon-reload
sudo systemctl enable web3-service
sudo systemctl start web3-service

# 检查服务状态
sudo systemctl status web3-service
```
  "Blockchain": {
    "NetworkUrl": "http://localhost:8545",
    "ChainId": 1337,
    "BillContractAddress": "0x...",
    "PaymentContractAddress": "0x..."
  },
  "DonutService": {
    "ReceiptServiceUrl": "http://localhost:8000",
    "PaymentServiceUrl": "http://localhost:8001"
  },
  "Web3Service": {
    "ServiceUrl": "http://localhost:5002"
  }
}
```

```bash
# 构建项目
dotnet build --configuration Release

# 发布项目
dotnet publish --configuration Release --output /var/www/wechatreceiptbot-api

# 创建systemd服务文件
sudo nano /etc/systemd/system/wechatreceiptbot-api.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=WeChat Receipt Bot API
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/wechatreceiptbot-api/WeChatReceiptBot.API.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=wechatreceiptbot-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

```bash
# 启动API服务
sudo systemctl daemon-reload
sudo systemctl enable wechatreceiptbot-api
sudo systemctl start wechatreceiptbot-api

# 检查服务状态
sudo systemctl status wechatreceiptbot-api
```

#### 2.3 部署Donut识别服务

```bash
# 部署单据识别服务
cd ../donut-receipt-service

# 创建Python虚拟环境
python3 -m venv venv
source venv/bin/activate

# 安装依赖
pip install -r requirements.txt

# 创建systemd服务文件
sudo nano /etc/systemd/system/donut-receipt-service.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=Donut Receipt Recognition Service
After=network.target

[Service]
Type=simple
ExecStart=/home/ubuntu/WeChatReceiptBot/donut-receipt-service/venv/bin/python /home/ubuntu/WeChatReceiptBot/donut-receipt-service/src/main.py
Restart=always
RestartSec=5
User=ubuntu
WorkingDirectory=/home/ubuntu/WeChatReceiptBot/donut-receipt-service
Environment=FLASK_ENV=production

[Install]
WantedBy=multi-user.target
```

```bash
# 部署支付记录识别服务
cd ../donut-payment-service

# 创建Python虚拟环境
python3 -m venv venv
source venv/bin/activate

# 安装依赖
pip install -r requirements.txt

# 创建systemd服务文件
sudo nano /etc/systemd/system/donut-payment-service.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=Donut Payment Recognition Service
After=network.target

[Service]
Type=simple
ExecStart=/home/ubuntu/WeChatReceiptBot/donut-payment-service/venv/bin/python /home/ubuntu/WeChatReceiptBot/donut-payment-service/src/main.py
Restart=always
RestartSec=5
User=ubuntu
WorkingDirectory=/home/ubuntu/WeChatReceiptBot/donut-payment-service
Environment=FLASK_ENV=production

[Install]
WantedBy=multi-user.target
```

```bash
# 启动Donut服务
sudo systemctl daemon-reload
sudo systemctl enable donut-receipt-service
sudo systemctl enable donut-payment-service
sudo systemctl start donut-receipt-service
sudo systemctl start donut-payment-service

# 检查服务状态
sudo systemctl status donut-receipt-service
sudo systemctl status donut-payment-service
```

#### 2.4 部署Web3服务

```bash
# 进入Web3服务目录
cd ../web3-service

# 创建Python虚拟环境
python3 -m venv venv
source venv/bin/activate

# 安装依赖
pip install -r requirements.txt

# 创建systemd服务文件
sudo nano /etc/systemd/system/web3-service.service
```

**服务配置文件内容：**
```ini
[Unit]
Description=Web3 Smart Contract Service
After=network.target

[Service]
Type=simple
ExecStart=/home/ubuntu/WeChatReceiptBot/web3-service/venv/bin/python /home/ubuntu/WeChatReceiptBot/web3-service/src/main.py
Restart=always
RestartSec=5
User=ubuntu
WorkingDirectory=/home/ubuntu/WeChatReceiptBot/web3-service
Environment=FLASK_ENV=production
Environment=WEB3_PROVIDER_URL=http://localhost:8545

[Install]
WantedBy=multi-user.target
```

```bash
# 启动Web3服务
sudo systemctl daemon-reload
sudo systemctl enable web3-service
sudo systemctl start web3-service

# 检查服务状态
sudo systemctl status web3-service
```

#### 2.5 部署前端应用

```bash
# 进入前端项目目录
cd ../wechat-receipt-frontend

# 安装依赖
npm install

# 配置API端点
nano .env.production
```

**环境配置文件内容（.env.production）：**
```
VITE_API_BASE_URL=https://your-domain.com/api
VITE_WEB3_SERVICE_URL=https://your-domain.com/api/web3
VITE_DONUT_RECEIPT_URL=https://your-domain.com/donut-receipt
VITE_DONUT_PAYMENT_URL=https://your-domain.com/donut-payment
```

```bash
# 构建生产版本
npm run build

# 复制构建文件到Web服务器目录
sudo cp -r dist/* /var/www/html/
```

### 第三步：配置Nginx反向代理

```bash
# 创建Nginx配置文件
sudo nano /etc/nginx/sites-available/wechatreceiptbot
```

**Nginx配置文件内容：**
```nginx
server {
    listen 80;
    server_name your-domain.com;

    # 前端静态文件
    location / {
        root /var/www/html;
        try_files $uri $uri/ /index.html;
    }

    # 后端API代理
    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    # Donut单据识别服务代理
    location /donut-receipt/ {
        proxy_pass http://localhost:8000/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Donut支付记录识别服务代理
    location /donut-payment/ {
        proxy_pass http://localhost:8001/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Web3服务代理
    location /web3/ {
        proxy_pass http://localhost:5002/api/web3/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# 启用站点配置
sudo ln -s /etc/nginx/sites-available/wechatreceiptbot /etc/nginx/sites-enabled/

# 测试Nginx配置
sudo nginx -t

# 重启Nginx
sudo systemctl restart nginx
```

### 第四步：SSL证书配置（可选但推荐）

```bash
# 安装Certbot
sudo apt install -y certbot python3-certbot-nginx

# 获取SSL证书
sudo certbot --nginx -d your-domain.com

# 设置自动续期
sudo crontab -e
# 添加以下行：
# 0 12 * * * /usr/bin/certbot renew --quiet
```

### 第五步：智能合约部署

#### 5.1 安装区块链开发工具

```bash
# 安装Hardhat
npm install -g hardhat

# 创建Hardhat项目
cd smart_contracts
npx hardhat init

# 安装依赖
npm install @openzeppelin/contracts
```

#### 5.2 配置Hardhat

**hardhat.config.js配置：**
```javascript
require("@nomiclabs/hardhat-waffle");

module.exports = {
  solidity: "0.8.19",
  networks: {
    localhost: {
      url: "http://127.0.0.1:8545"
    },
    testnet: {
      url: "https://rpc.testnet.fantom.network",
      accounts: ["your-private-key-here"]
    }
  }
};
```

#### 5.3 部署智能合约

```bash
# 编译合约
npx hardhat compile

# 部署到本地网络
npx hardhat run scripts/deploy.js --network localhost

# 部署到测试网络
npx hardhat run scripts/deploy.js --network testnet
```

**部署脚本示例（scripts/deploy.js）：**
```javascript
async function main() {
  const BillContract = await ethers.getContractFactory("BillContract");
  const billContract = await BillContract.deploy();
  await billContract.deployed();
  console.log("BillContract deployed to:", billContract.address);

  const PaymentContract = await ethers.getContractFactory("PaymentContract");
  const paymentContract = await PaymentContract.deploy();
  await paymentContract.deployed();
  console.log("PaymentContract deployed to:", paymentContract.address);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
```

### 第六步：数据库初始化

#### 6.1 NebulaGraph初始化

```bash
# 连接到NebulaGraph
nebula-console -addr 127.0.0.1 -port 9669 -u root -p nebula

# 创建图空间
CREATE SPACE wechatreceiptbot(partition_num=10, replica_factor=1, vid_type=FIXED_STRING(64));
USE wechatreceiptbot;

# 创建标签
CREATE TAG User(userId string, username string, nickname string, walletAddress string, registrationDate datetime);
CREATE TAG Group(groupId string, groupName string, description string, createdAt datetime, currency string);
CREATE TAG Bill(billId string, billName string, amount double, currency string, createdAt datetime, status string);
CREATE TAG Transaction(transactionId string, amount double, description string, transactionType string, timestamp datetime);

# 创建边类型
CREATE EDGE BELONGS_TO(role string, joinedAt datetime);
CREATE EDGE CREATED(createdAt datetime);
CREATE EDGE PART_OF(addedAt datetime);
CREATE EDGE PAID(paidAt datetime, paymentMethod string);
CREATE EDGE RECEIVED(receivedAt datetime);
CREATE EDGE FRIENDS_WITH(since datetime, strength int);
```

#### 6.2 Apache Doris初始化

```sql
-- 连接到Doris
mysql -h 127.0.0.1 -P 9030 -u root

-- 创建数据库
CREATE DATABASE wechatreceiptbot;
USE wechatreceiptbot;

-- 创建事实表
CREATE TABLE fact_transactions (
    transaction_id VARCHAR(64),
    bill_id VARCHAR(64),
    user_id VARCHAR(64),
    amount DECIMAL(18,2),
    transaction_type VARCHAR(32),
    timestamp DATETIME,
    payment_method VARCHAR(64),
    status VARCHAR(32)
) DISTRIBUTED BY HASH(transaction_id) BUCKETS 10;

CREATE TABLE fact_bills (
    bill_id VARCHAR(64),
    group_id VARCHAR(64),
    creator_id VARCHAR(64),
    total_amount DECIMAL(18,2),
    settled_amount DECIMAL(18,2),
    created_at DATETIME,
    settled_at DATETIME,
    status VARCHAR(32)
) DISTRIBUTED BY HASH(bill_id) BUCKETS 10;

-- 创建维度表
CREATE TABLE dim_users (
    user_id VARCHAR(64),
    username VARCHAR(128),
    nickname VARCHAR(128),
    registration_date DATETIME,
    last_active_date DATETIME,
    wallet_address VARCHAR(128)
) DISTRIBUTED BY HASH(user_id) BUCKETS 10;
```

### 第七步：系统测试

#### 7.1 健康检查

```bash
# 检查所有服务状态
sudo systemctl status wechatreceiptbot-api
sudo systemctl status donut-receipt-service
sudo systemctl status donut-payment-service
sudo systemctl status web3-service
sudo systemctl status nginx

# 检查端口监听
netstat -tlnp | grep -E ':(5000|8000|8001|5002|80|443)'

# 测试API端点
curl -X GET http://localhost:5000/api/health
curl -X GET http://localhost:8000/api/health
curl -X GET http://localhost:8001/api/health
curl -X GET http://localhost:5002/api/web3/health
```

#### 7.2 功能测试

```bash
# 测试用户注册
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test123!","email":"test@example.com"}'

# 测试单据识别
curl -X POST http://localhost:8000/api/recognize/receipt \
  -H "Content-Type: application/json" \
  -d '{"image":"base64-encoded-image-data"}'

# 测试Web3功能
curl -X POST http://localhost:5002/api/web3/account/create \
  -H "Content-Type: application/json"
```

### 第八步：监控和日志

#### 8.1 配置日志

```bash
# 查看API服务日志
sudo journalctl -u wechatreceiptbot-api -f

# 查看Donut服务日志
sudo journalctl -u donut-receipt-service -f
sudo journalctl -u donut-payment-service -f

# 查看Web3服务日志
sudo journalctl -u web3-service -f

# 查看Nginx日志
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

#### 8.2 性能监控

```bash
# 安装监控工具
sudo apt install -y htop iotop nethogs

# 监控系统资源
htop
iotop
nethogs

# 监控数据库性能
# NebulaGraph监控
nebula-stats-exporter

# Doris监控
# 访问 http://localhost:8030 查看Doris Web UI
```

### 第九步：备份和恢复

#### 9.1 数据库备份

```bash
# NebulaGraph备份
nebula-br backup --meta "127.0.0.1:9559" --storage "127.0.0.1:9779" --backup_name "backup_$(date +%Y%m%d_%H%M%S)"

# Doris备份
mysqldump -h 127.0.0.1 -P 9030 -u root wechatreceiptbot > wechatreceiptbot_backup_$(date +%Y%m%d_%H%M%S).sql
```

#### 9.2 代码备份

```bash
# 创建代码备份
tar -czf wechatreceiptbot_code_backup_$(date +%Y%m%d_%H%M%S).tar.gz /home/ubuntu/WeChatReceiptBot/

# 备份配置文件
tar -czf wechatreceiptbot_config_backup_$(date +%Y%m%d_%H%M%S).tar.gz \
  /etc/nginx/sites-available/wechatreceiptbot \
  /etc/systemd/system/wechatreceiptbot-* \
  /var/www/wechatreceiptbot-api/appsettings.Production.json
```

### 第十步：安全加固

#### 10.1 防火墙配置

```bash
# 安装并配置UFW防火墙
sudo ufw enable
sudo ufw default deny incoming
sudo ufw default allow outgoing

# 允许必要端口
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# 限制内部服务端口访问
sudo ufw allow from 127.0.0.1 to any port 5000
sudo ufw allow from 127.0.0.1 to any port 8000
sudo ufw allow from 127.0.0.1 to any port 8001
sudo ufw allow from 127.0.0.1 to any port 5002
```

#### 10.2 系统安全

```bash
# 更新系统安全补丁
sudo apt update && sudo apt upgrade -y

# 配置自动安全更新
sudo apt install -y unattended-upgrades
sudo dpkg-reconfigure -plow unattended-upgrades

# 配置fail2ban防止暴力破解
sudo apt install -y fail2ban
sudo systemctl enable fail2ban
sudo systemctl start fail2ban
```

## 故障排除

### 常见问题

1. **API服务无法启动**
   - 检查端口是否被占用：`netstat -tlnp | grep 5000`
   - 检查配置文件格式：`dotnet --info`
   - 查看详细错误日志：`sudo journalctl -u wechatreceiptbot-api -n 50`

2. **数据库连接失败**
   - 检查NebulaGraph服务状态：`sudo systemctl status nebula-*`
   - 检查Doris服务状态：`ps aux | grep doris`
   - 验证连接字符串配置

3. **前端无法访问API**
   - 检查Nginx配置：`sudo nginx -t`
   - 检查CORS设置
   - 验证API端点URL

4. **智能合约部署失败**
   - 检查区块链网络连接
   - 验证私钥和账户余额
   - 检查合约代码语法

### 性能优化

1. **数据库优化**
   - 配置NebulaGraph内存参数
   - 优化Doris查询性能
   - 定期清理过期数据

2. **应用优化**
   - 启用API响应缓存
   - 优化前端资源加载
   - 配置CDN加速

3. **系统优化**
   - 调整系统内核参数
   - 优化磁盘I/O性能
   - 配置负载均衡

## 维护指南

### 日常维护

1. **监控检查**
   - 每日检查服务状态
   - 监控系统资源使用
   - 查看错误日志

2. **数据备份**
   - 每日自动备份数据库
   - 每周备份代码和配置
   - 定期测试恢复流程

3. **安全更新**
   - 每月更新系统补丁
   - 定期更新依赖包
   - 监控安全漏洞

### 升级指南

1. **应用升级**
   - 备份当前版本
   - 测试新版本兼容性
   - 逐步滚动升级

2. **数据库升级**
   - 制定升级计划
   - 执行数据迁移
   - 验证数据完整性

## 联系支持

如果在部署过程中遇到问题，请联系技术支持：

- 邮箱：support@wechatreceiptbot.com
- 文档：https://docs.wechatreceiptbot.com
- GitHub Issues：https://github.com/wechatreceiptbot/issues

## 许可证

本项目采用MIT许可证。详情请参阅LICENSE文件。

