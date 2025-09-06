from flask import Blueprint, request, jsonify
from flask_cors import cross_origin
from web3 import Web3
from eth_account import Account
import json
import os
from datetime import datetime

web3_bp = Blueprint('web3', __name__)

# Web3 configuration
# For development, using Ganache or local blockchain
WEB3_PROVIDER_URL = os.getenv('WEB3_PROVIDER_URL', 'http://localhost:8545')
PRIVATE_KEY = os.getenv('PRIVATE_KEY', '0x' + '0' * 64)  # Default private key for development

# Initialize Web3
try:
    w3 = Web3(Web3.HTTPProvider(WEB3_PROVIDER_URL))
    if w3.is_connected():
        print(f"Connected to Web3 provider: {WEB3_PROVIDER_URL}")
    else:
        print(f"Failed to connect to Web3 provider: {WEB3_PROVIDER_URL}")
        w3 = None
except Exception as e:
    print(f"Web3 connection error: {e}")
    w3 = None

# Smart contract ABIs (simplified for demo)
BILL_CONTRACT_ABI = [
    {
        "inputs": [
            {"name": "billId", "type": "string"},
            {"name": "billName", "type": "string"},
            {"name": "description", "type": "string"},
            {"name": "currency", "type": "string"}
        ],
        "name": "createBill",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    },
    {
        "inputs": [
            {"name": "transactionId", "type": "string"},
            {"name": "billId", "type": "string"},
            {"name": "amount", "type": "uint256"},
            {"name": "description", "type": "string"},
            {"name": "transactionType", "type": "string"},
            {"name": "beneficiaries", "type": "address[]"},
            {"name": "beneficiaryAmounts", "type": "uint256[]"}
        ],
        "name": "addTransaction",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    },
    {
        "inputs": [{"name": "billId", "type": "string"}],
        "name": "getBill",
        "outputs": [
            {"name": "", "type": "string"},
            {"name": "", "type": "string"},
            {"name": "", "type": "string"},
            {"name": "", "type": "address"},
            {"name": "", "type": "uint256"},
            {"name": "", "type": "uint256"},
            {"name": "", "type": "string"},
            {"name": "", "type": "bool"},
            {"name": "", "type": "uint256"},
            {"name": "", "type": "address[]"}
        ],
        "stateMutability": "view",
        "type": "function"
    }
]

PAYMENT_CONTRACT_ABI = [
    {
        "inputs": [
            {"name": "paymentId", "type": "string"},
            {"name": "transactionId", "type": "string"},
            {"name": "receiver", "type": "address"},
            {"name": "amount", "type": "uint256"},
            {"name": "currency", "type": "string"},
            {"name": "paymentMethod", "type": "string"},
            {"name": "paymentDate", "type": "uint256"},
            {"name": "notes", "type": "string"},
            {"name": "imageHash", "type": "string"}
        ],
        "name": "recordPayment",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    },
    {
        "inputs": [{"name": "paymentId", "type": "string"}],
        "name": "getPaymentRecord",
        "outputs": [
            {"name": "", "type": "string"},
            {"name": "", "type": "string"},
            {"name": "", "type": "address"},
            {"name": "", "type": "address"},
            {"name": "", "type": "uint256"},
            {"name": "", "type": "string"},
            {"name": "", "type": "string"},
            {"name": "", "type": "uint256"},
            {"name": "", "type": "uint256"},
            {"name": "", "type": "string"},
            {"name": "", "type": "string"},
            {"name": "", "type": "string"},
            {"name": "", "type": "bool"},
            {"name": "", "type": "address"},
            {"name": "", "type": "uint256"}
        ],
        "stateMutability": "view",
        "type": "function"
    }
]

# Contract addresses (would be set after deployment)
BILL_CONTRACT_ADDRESS = os.getenv('BILL_CONTRACT_ADDRESS', '0x' + '0' * 40)
PAYMENT_CONTRACT_ADDRESS = os.getenv('PAYMENT_CONTRACT_ADDRESS', '0x' + '0' * 40)

@web3_bp.route('/health', methods=['GET'])
@cross_origin()
def health_check():
    """Health check endpoint"""
    return jsonify({
        'success': True,
        'message': 'Web3 service is running',
        'web3_connected': w3 is not None and w3.is_connected() if w3 else False,
        'provider_url': WEB3_PROVIDER_URL
    })

@web3_bp.route('/account/create', methods=['POST'])
@cross_origin()
def create_account():
    """Create a new Ethereum account"""
    try:
        # Create a new account
        account = Account.create()
        
        return jsonify({
            'success': True,
            'data': {
                'address': account.address,
                'private_key': account.key.hex(),
                'public_key': account._key_obj.public_key.to_hex()
            },
            'message': 'Account created successfully'
        })
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to create account'
        }), 500

@web3_bp.route('/account/balance/<address>', methods=['GET'])
@cross_origin()
def get_balance(address):
    """Get account balance"""
    try:
        if not w3 or not w3.is_connected():
            return jsonify({
                'success': False,
                'error': 'Web3 not connected',
                'message': 'Cannot connect to blockchain'
            }), 500
        
        # Get balance in Wei
        balance_wei = w3.eth.get_balance(address)
        # Convert to Ether
        balance_eth = w3.from_wei(balance_wei, 'ether')
        
        return jsonify({
            'success': True,
            'data': {
                'address': address,
                'balance_wei': str(balance_wei),
                'balance_eth': str(balance_eth)
            },
            'message': 'Balance retrieved successfully'
        })
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to get balance'
        }), 500

@web3_bp.route('/bill/create', methods=['POST'])
@cross_origin()
def create_bill():
    """Create a new bill on blockchain"""
    try:
        data = request.get_json()
        
        # Validate required fields
        required_fields = ['billId', 'billName', 'description', 'currency']
        for field in required_fields:
            if field not in data:
                return jsonify({
                    'success': False,
                    'error': f'Missing required field: {field}',
                    'message': 'Invalid request data'
                }), 400
        
        # For demo purposes, simulate blockchain transaction
        bill_data = {
            'billId': data['billId'],
            'billName': data['billName'],
            'description': data['description'],
            'currency': data['currency'],
            'creator': data.get('creator', '0x' + '0' * 40),
            'totalAmount': 0,
            'settledAmount': 0,
            'isSettled': False,
            'createdAt': int(datetime.now().timestamp()),
            'transactionHash': '0x' + 'a' * 64,  # Mock transaction hash
            'blockNumber': 12345,
            'gasUsed': 150000
        }
        
        return jsonify({
            'success': True,
            'data': bill_data,
            'message': 'Bill created successfully on blockchain'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to create bill'
        }), 500

@web3_bp.route('/bill/<bill_id>', methods=['GET'])
@cross_origin()
def get_bill(bill_id):
    """Get bill information from blockchain"""
    try:
        # For demo purposes, return mock data
        bill_data = {
            'billId': bill_id,
            'billName': f'Bill {bill_id}',
            'description': 'Sample bill description',
            'creator': '0x' + '1' * 40,
            'totalAmount': 1000,
            'settledAmount': 500,
            'currency': 'CNY',
            'isSettled': False,
            'createdAt': int(datetime.now().timestamp()),
            'members': ['0x' + '1' * 40, '0x' + '2' * 40]
        }
        
        return jsonify({
            'success': True,
            'data': bill_data,
            'message': 'Bill retrieved successfully'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to get bill'
        }), 500

@web3_bp.route('/transaction/add', methods=['POST'])
@cross_origin()
def add_transaction():
    """Add a transaction to a bill"""
    try:
        data = request.get_json()
        
        # Validate required fields
        required_fields = ['transactionId', 'billId', 'amount', 'description', 'transactionType']
        for field in required_fields:
            if field not in data:
                return jsonify({
                    'success': False,
                    'error': f'Missing required field: {field}',
                    'message': 'Invalid request data'
                }), 400
        
        # For demo purposes, simulate blockchain transaction
        transaction_data = {
            'transactionId': data['transactionId'],
            'billId': data['billId'],
            'payer': data.get('payer', '0x' + '0' * 40),
            'amount': data['amount'],
            'description': data['description'],
            'transactionType': data['transactionType'],
            'timestamp': int(datetime.now().timestamp()),
            'isSettled': False,
            'beneficiaries': data.get('beneficiaries', []),
            'transactionHash': '0x' + 'b' * 64,  # Mock transaction hash
            'blockNumber': 12346,
            'gasUsed': 120000
        }
        
        return jsonify({
            'success': True,
            'data': transaction_data,
            'message': 'Transaction added successfully to blockchain'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to add transaction'
        }), 500

@web3_bp.route('/payment/record', methods=['POST'])
@cross_origin()
def record_payment():
    """Record a payment on blockchain"""
    try:
        data = request.get_json()
        
        # Validate required fields
        required_fields = ['paymentId', 'receiver', 'amount', 'currency', 'paymentMethod']
        for field in required_fields:
            if field not in data:
                return jsonify({
                    'success': False,
                    'error': f'Missing required field: {field}',
                    'message': 'Invalid request data'
                }), 400
        
        # For demo purposes, simulate blockchain transaction
        payment_data = {
            'paymentId': data['paymentId'],
            'transactionId': data.get('transactionId', ''),
            'payer': data.get('payer', '0x' + '0' * 40),
            'receiver': data['receiver'],
            'amount': data['amount'],
            'currency': data['currency'],
            'paymentMethod': data['paymentMethod'],
            'paymentDate': data.get('paymentDate', int(datetime.now().timestamp())),
            'createdAt': int(datetime.now().timestamp()),
            'status': 'Completed',
            'notes': data.get('notes', ''),
            'imageHash': data.get('imageHash', ''),
            'isVerified': False,
            'transactionHash': '0x' + 'c' * 64,  # Mock transaction hash
            'blockNumber': 12347,
            'gasUsed': 100000
        }
        
        return jsonify({
            'success': True,
            'data': payment_data,
            'message': 'Payment recorded successfully on blockchain'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to record payment'
        }), 500

@web3_bp.route('/payment/<payment_id>', methods=['GET'])
@cross_origin()
def get_payment(payment_id):
    """Get payment record from blockchain"""
    try:
        # For demo purposes, return mock data
        payment_data = {
            'paymentId': payment_id,
            'transactionId': 'tx_001',
            'payer': '0x' + '1' * 40,
            'receiver': '0x' + '2' * 40,
            'amount': 100,
            'currency': 'CNY',
            'paymentMethod': 'WeChat Pay',
            'paymentDate': int(datetime.now().timestamp()),
            'createdAt': int(datetime.now().timestamp()),
            'status': 'Completed',
            'notes': 'Payment for lunch',
            'imageHash': 'hash123',
            'isVerified': True,
            'verifiedBy': '0x' + '2' * 40,
            'verifiedAt': int(datetime.now().timestamp())
        }
        
        return jsonify({
            'success': True,
            'data': payment_data,
            'message': 'Payment record retrieved successfully'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to get payment record'
        }), 500

@web3_bp.route('/contract/deploy', methods=['POST'])
@cross_origin()
def deploy_contract():
    """Deploy smart contracts (for development/testing)"""
    try:
        data = request.get_json()
        contract_type = data.get('contractType', 'bill')
        
        # For demo purposes, simulate contract deployment
        contract_data = {
            'contractType': contract_type,
            'contractAddress': '0x' + 'd' * 40,
            'transactionHash': '0x' + 'e' * 64,
            'blockNumber': 12348,
            'gasUsed': 2000000,
            'deployedAt': int(datetime.now().timestamp())
        }
        
        return jsonify({
            'success': True,
            'data': contract_data,
            'message': f'{contract_type.title()} contract deployed successfully'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to deploy contract'
        }), 500

@web3_bp.route('/gas/estimate', methods=['POST'])
@cross_origin()
def estimate_gas():
    """Estimate gas for a transaction"""
    try:
        data = request.get_json()
        operation = data.get('operation', 'createBill')
        
        # Mock gas estimates for different operations
        gas_estimates = {
            'createBill': 150000,
            'addTransaction': 120000,
            'recordPayment': 100000,
            'verifyPayment': 50000,
            'settleBill': 80000
        }
        
        estimated_gas = gas_estimates.get(operation, 100000)
        gas_price = 20000000000  # 20 Gwei
        estimated_cost = estimated_gas * gas_price
        
        return jsonify({
            'success': True,
            'data': {
                'operation': operation,
                'estimatedGas': estimated_gas,
                'gasPrice': gas_price,
                'estimatedCostWei': estimated_cost,
                'estimatedCostEth': str(w3.from_wei(estimated_cost, 'ether')) if w3 else '0'
            },
            'message': 'Gas estimation completed'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'message': 'Failed to estimate gas'
        }), 500

