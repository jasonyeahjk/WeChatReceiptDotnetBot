// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

/**
 * @title PaymentContract
 * @dev Smart contract for managing payment records and verification
 */
contract PaymentContract {
    struct PaymentRecord {
        string paymentId;
        string transactionId; // Optional link to transaction
        address payer;
        address receiver;
        uint256 amount;
        string currency;
        string paymentMethod;
        uint256 paymentDate;
        uint256 createdAt;
        string status; // "Pending", "Completed", "Failed"
        string notes;
        string imageHash; // Hash of payment screenshot
        bool isVerified;
        address verifiedBy;
        uint256 verifiedAt;
    }
    
    mapping(string => PaymentRecord) public paymentRecords;
    mapping(address => string[]) public userPayments;
    mapping(address => string[]) public userReceivedPayments;
    
    string[] public allPaymentIds;
    
    event PaymentRecorded(
        string indexed paymentId, 
        address indexed payer, 
        address indexed receiver, 
        uint256 amount
    );
    event PaymentVerified(string indexed paymentId, address indexed verifier);
    event PaymentStatusUpdated(string indexed paymentId, string status);
    
    modifier paymentExists(string memory paymentId) {
        require(bytes(paymentRecords[paymentId].paymentId).length > 0, "Payment record does not exist");
        _;
    }
    
    modifier onlyPayerOrReceiver(string memory paymentId) {
        PaymentRecord storage payment = paymentRecords[paymentId];
        require(
            msg.sender == payment.payer || msg.sender == payment.receiver,
            "Only payer or receiver can perform this action"
        );
        _;
    }
    
    /**
     * @dev Record a new payment
     */
    function recordPayment(
        string memory paymentId,
        string memory transactionId,
        address receiver,
        uint256 amount,
        string memory currency,
        string memory paymentMethod,
        uint256 paymentDate,
        string memory notes,
        string memory imageHash
    ) public {
        require(bytes(paymentRecords[paymentId].paymentId).length == 0, "Payment record already exists");
        require(receiver != address(0), "Invalid receiver address");
        require(amount > 0, "Payment amount must be greater than 0");
        
        PaymentRecord storage newPayment = paymentRecords[paymentId];
        newPayment.paymentId = paymentId;
        newPayment.transactionId = transactionId;
        newPayment.payer = msg.sender;
        newPayment.receiver = receiver;
        newPayment.amount = amount;
        newPayment.currency = currency;
        newPayment.paymentMethod = paymentMethod;
        newPayment.paymentDate = paymentDate;
        newPayment.createdAt = block.timestamp;
        newPayment.status = "Completed";
        newPayment.notes = notes;
        newPayment.imageHash = imageHash;
        newPayment.isVerified = false;
        
        allPaymentIds.push(paymentId);
        userPayments[msg.sender].push(paymentId);
        userReceivedPayments[receiver].push(paymentId);
        
        emit PaymentRecorded(paymentId, msg.sender, receiver, amount);
    }
    
    /**
     * @dev Verify a payment record
     */
    function verifyPayment(string memory paymentId) 
        public 
        paymentExists(paymentId) 
        onlyPayerOrReceiver(paymentId) 
    {
        PaymentRecord storage payment = paymentRecords[paymentId];
        require(!payment.isVerified, "Payment already verified");
        
        payment.isVerified = true;
        payment.verifiedBy = msg.sender;
        payment.verifiedAt = block.timestamp;
        
        emit PaymentVerified(paymentId, msg.sender);
    }
    
    /**
     * @dev Update payment status
     */
    function updatePaymentStatus(string memory paymentId, string memory newStatus) 
        public 
        paymentExists(paymentId) 
        onlyPayerOrReceiver(paymentId) 
    {
        PaymentRecord storage payment = paymentRecords[paymentId];
        payment.status = newStatus;
        
        emit PaymentStatusUpdated(paymentId, newStatus);
    }
    
    /**
     * @dev Get payment record information
     */
    function getPaymentRecord(string memory paymentId) 
        public 
        view 
        paymentExists(paymentId) 
        returns (
            string memory,
            string memory,
            address,
            address,
            uint256,
            string memory,
            string memory,
            uint256,
            uint256,
            string memory,
            string memory,
            string memory,
            bool,
            address,
            uint256
        ) 
    {
        PaymentRecord storage payment = paymentRecords[paymentId];
        return (
            payment.paymentId,
            payment.transactionId,
            payment.payer,
            payment.receiver,
            payment.amount,
            payment.currency,
            payment.paymentMethod,
            payment.paymentDate,
            payment.createdAt,
            payment.status,
            payment.notes,
            payment.imageHash,
            payment.isVerified,
            payment.verifiedBy,
            payment.verifiedAt
        );
    }
    
    /**
     * @dev Get user's payment records (as payer)
     */
    function getUserPayments(address user) public view returns (string[] memory) {
        return userPayments[user];
    }
    
    /**
     * @dev Get user's received payment records (as receiver)
     */
    function getUserReceivedPayments(address user) public view returns (string[] memory) {
        return userReceivedPayments[user];
    }
    
    /**
     * @dev Get all payment records
     */
    function getAllPayments() public view returns (string[] memory) {
        return allPaymentIds;
    }
    
    /**
     * @dev Get payment records by transaction ID
     */
    function getPaymentsByTransaction(string memory transactionId) 
        public 
        view 
        returns (string[] memory) 
    {
        string[] memory result = new string[](allPaymentIds.length);
        uint256 count = 0;
        
        for (uint256 i = 0; i < allPaymentIds.length; i++) {
            if (keccak256(bytes(paymentRecords[allPaymentIds[i]].transactionId)) == keccak256(bytes(transactionId))) {
                result[count] = allPaymentIds[i];
                count++;
            }
        }
        
        // Resize array to actual count
        string[] memory finalResult = new string[](count);
        for (uint256 i = 0; i < count; i++) {
            finalResult[i] = result[i];
        }
        
        return finalResult;
    }
    
    /**
     * @dev Get total amount paid by user
     */
    function getUserTotalPaid(address user) public view returns (uint256) {
        uint256 total = 0;
        string[] memory payments = userPayments[user];
        
        for (uint256 i = 0; i < payments.length; i++) {
            PaymentRecord storage payment = paymentRecords[payments[i]];
            if (keccak256(bytes(payment.status)) == keccak256(bytes("Completed"))) {
                total += payment.amount;
            }
        }
        
        return total;
    }
    
    /**
     * @dev Get total amount received by user
     */
    function getUserTotalReceived(address user) public view returns (uint256) {
        uint256 total = 0;
        string[] memory payments = userReceivedPayments[user];
        
        for (uint256 i = 0; i < payments.length; i++) {
            PaymentRecord storage payment = paymentRecords[payments[i]];
            if (keccak256(bytes(payment.status)) == keccak256(bytes("Completed"))) {
                total += payment.amount;
            }
        }
        
        return total;
    }
    
    /**
     * @dev Check if payment is verified
     */
    function isPaymentVerified(string memory paymentId) 
        public 
        view 
        paymentExists(paymentId) 
        returns (bool) 
    {
        return paymentRecords[paymentId].isVerified;
    }
    
    /**
     * @dev Get payment count for user
     */
    function getUserPaymentCount(address user) public view returns (uint256 sent, uint256 received) {
        sent = userPayments[user].length;
        received = userReceivedPayments[user].length;
    }
}

