// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

/**
 * @title BillContract
 * @dev Smart contract for managing group bills and expense sharing
 */
contract BillContract {
    struct Bill {
        string billId;
        string billName;
        string description;
        address creator;
        uint256 totalAmount;
        uint256 settledAmount;
        string currency;
        bool isSettled;
        uint256 createdAt;
        mapping(address => uint256) memberShares;
        mapping(address => uint256) memberPaid;
        address[] members;
    }
    
    struct Transaction {
        string transactionId;
        string billId;
        address payer;
        uint256 amount;
        string description;
        string transactionType; // "expense" or "payment"
        uint256 timestamp;
        bool isSettled;
        address[] beneficiaries;
        mapping(address => uint256) beneficiaryAmounts;
    }
    
    mapping(string => Bill) public bills;
    mapping(string => Transaction) public transactions;
    mapping(address => string[]) public userBills;
    mapping(address => string[]) public userTransactions;
    
    string[] public allBillIds;
    string[] public allTransactionIds;
    
    event BillCreated(string indexed billId, address indexed creator, uint256 totalAmount);
    event TransactionAdded(string indexed transactionId, string indexed billId, address indexed payer, uint256 amount);
    event BillSettled(string indexed billId, uint256 totalAmount, uint256 settledAmount);
    event PaymentMade(string indexed billId, address indexed payer, address indexed receiver, uint256 amount);
    
    modifier onlyBillCreator(string memory billId) {
        require(bills[billId].creator == msg.sender, "Only bill creator can perform this action");
        _;
    }
    
    modifier billExists(string memory billId) {
        require(bytes(bills[billId].billId).length > 0, "Bill does not exist");
        _;
    }
    
    modifier transactionExists(string memory transactionId) {
        require(bytes(transactions[transactionId].transactionId).length > 0, "Transaction does not exist");
        _;
    }
    
    /**
     * @dev Create a new bill
     */
    function createBill(
        string memory billId,
        string memory billName,
        string memory description,
        string memory currency
    ) public {
        require(bytes(bills[billId].billId).length == 0, "Bill already exists");
        
        Bill storage newBill = bills[billId];
        newBill.billId = billId;
        newBill.billName = billName;
        newBill.description = description;
        newBill.creator = msg.sender;
        newBill.totalAmount = 0;
        newBill.settledAmount = 0;
        newBill.currency = currency;
        newBill.isSettled = false;
        newBill.createdAt = block.timestamp;
        newBill.members.push(msg.sender);
        
        allBillIds.push(billId);
        userBills[msg.sender].push(billId);
        
        emit BillCreated(billId, msg.sender, 0);
    }
    
    /**
     * @dev Add a member to a bill
     */
    function addMemberToBill(string memory billId, address member) 
        public 
        billExists(billId) 
        onlyBillCreator(billId) 
    {
        Bill storage bill = bills[billId];
        
        // Check if member already exists
        for (uint i = 0; i < bill.members.length; i++) {
            if (bill.members[i] == member) {
                revert("Member already exists in bill");
            }
        }
        
        bill.members.push(member);
        userBills[member].push(billId);
    }
    
    /**
     * @dev Add a transaction to a bill
     */
    function addTransaction(
        string memory transactionId,
        string memory billId,
        uint256 amount,
        string memory description,
        string memory transactionType,
        address[] memory beneficiaries,
        uint256[] memory beneficiaryAmounts
    ) public billExists(billId) {
        require(bytes(transactions[transactionId].transactionId).length == 0, "Transaction already exists");
        require(beneficiaries.length == beneficiaryAmounts.length, "Beneficiaries and amounts length mismatch");
        
        // Verify total beneficiary amounts equal transaction amount
        uint256 totalBeneficiaryAmount = 0;
        for (uint i = 0; i < beneficiaryAmounts.length; i++) {
            totalBeneficiaryAmount += beneficiaryAmounts[i];
        }
        require(totalBeneficiaryAmount == amount, "Total beneficiary amounts must equal transaction amount");
        
        Transaction storage newTransaction = transactions[transactionId];
        newTransaction.transactionId = transactionId;
        newTransaction.billId = billId;
        newTransaction.payer = msg.sender;
        newTransaction.amount = amount;
        newTransaction.description = description;
        newTransaction.transactionType = transactionType;
        newTransaction.timestamp = block.timestamp;
        newTransaction.isSettled = false;
        newTransaction.beneficiaries = beneficiaries;
        
        // Set beneficiary amounts
        for (uint i = 0; i < beneficiaries.length; i++) {
            newTransaction.beneficiaryAmounts[beneficiaries[i]] = beneficiaryAmounts[i];
        }
        
        // Update bill total amount
        Bill storage bill = bills[billId];
        bill.totalAmount += amount;
        
        // Update member shares
        for (uint i = 0; i < beneficiaries.length; i++) {
            bill.memberShares[beneficiaries[i]] += beneficiaryAmounts[i];
        }
        
        // Update payer's paid amount
        bill.memberPaid[msg.sender] += amount;
        
        allTransactionIds.push(transactionId);
        userTransactions[msg.sender].push(transactionId);
        
        emit TransactionAdded(transactionId, billId, msg.sender, amount);
    }
    
    /**
     * @dev Make a payment to settle debts
     */
    function makePayment(
        string memory billId,
        address receiver,
        uint256 amount
    ) public payable billExists(billId) {
        require(amount > 0, "Payment amount must be greater than 0");
        require(msg.value >= amount, "Insufficient payment amount");
        
        Bill storage bill = bills[billId];
        
        // Update settled amount
        bill.settledAmount += amount;
        bill.memberPaid[msg.sender] += amount;
        
        // Transfer payment to receiver
        payable(receiver).transfer(amount);
        
        // Return excess payment
        if (msg.value > amount) {
            payable(msg.sender).transfer(msg.value - amount);
        }
        
        emit PaymentMade(billId, msg.sender, receiver, amount);
    }
    
    /**
     * @dev Settle a transaction
     */
    function settleTransaction(string memory transactionId) 
        public 
        transactionExists(transactionId) 
    {
        Transaction storage transaction = transactions[transactionId];
        require(!transaction.isSettled, "Transaction already settled");
        
        transaction.isSettled = true;
        
        // Check if all transactions in the bill are settled
        Bill storage bill = bills[transaction.billId];
        if (bill.settledAmount >= bill.totalAmount) {
            bill.isSettled = true;
            emit BillSettled(transaction.billId, bill.totalAmount, bill.settledAmount);
        }
    }
    
    /**
     * @dev Get bill information
     */
    function getBill(string memory billId) 
        public 
        view 
        billExists(billId) 
        returns (
            string memory,
            string memory,
            string memory,
            address,
            uint256,
            uint256,
            string memory,
            bool,
            uint256,
            address[] memory
        ) 
    {
        Bill storage bill = bills[billId];
        return (
            bill.billId,
            bill.billName,
            bill.description,
            bill.creator,
            bill.totalAmount,
            bill.settledAmount,
            bill.currency,
            bill.isSettled,
            bill.createdAt,
            bill.members
        );
    }
    
    /**
     * @dev Get member's share and paid amount for a bill
     */
    function getMemberBillInfo(string memory billId, address member) 
        public 
        view 
        billExists(billId) 
        returns (uint256 share, uint256 paid, int256 balance) 
    {
        Bill storage bill = bills[billId];
        share = bill.memberShares[member];
        paid = bill.memberPaid[member];
        balance = int256(paid) - int256(share);
    }
    
    /**
     * @dev Get transaction information
     */
    function getTransaction(string memory transactionId) 
        public 
        view 
        transactionExists(transactionId) 
        returns (
            string memory,
            string memory,
            address,
            uint256,
            string memory,
            string memory,
            uint256,
            bool,
            address[] memory
        ) 
    {
        Transaction storage transaction = transactions[transactionId];
        return (
            transaction.transactionId,
            transaction.billId,
            transaction.payer,
            transaction.amount,
            transaction.description,
            transaction.transactionType,
            transaction.timestamp,
            transaction.isSettled,
            transaction.beneficiaries
        );
    }
    
    /**
     * @dev Get beneficiary amount for a transaction
     */
    function getTransactionBeneficiaryAmount(string memory transactionId, address beneficiary) 
        public 
        view 
        transactionExists(transactionId) 
        returns (uint256) 
    {
        return transactions[transactionId].beneficiaryAmounts[beneficiary];
    }
    
    /**
     * @dev Get user's bills
     */
    function getUserBills(address user) public view returns (string[] memory) {
        return userBills[user];
    }
    
    /**
     * @dev Get user's transactions
     */
    function getUserTransactions(address user) public view returns (string[] memory) {
        return userTransactions[user];
    }
    
    /**
     * @dev Get all bills
     */
    function getAllBills() public view returns (string[] memory) {
        return allBillIds;
    }
    
    /**
     * @dev Get all transactions
     */
    function getAllTransactions() public view returns (string[] memory) {
        return allTransactionIds;
    }
}

