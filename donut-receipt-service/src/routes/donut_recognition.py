import base64
import json
import io
from PIL import Image
from flask import Blueprint, request, jsonify
from flask_cors import cross_origin
import logging

# Create blueprint
donut_bp = Blueprint('donut', __name__)

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def decode_base64_image(base64_string):
    """Decode base64 string to PIL Image"""
    try:
        # Remove data URL prefix if present
        if base64_string.startswith('data:image'):
            base64_string = base64_string.split(',')[1]
        
        # Decode base64
        image_data = base64.b64decode(base64_string)
        image = Image.open(io.BytesIO(image_data))
        return image
    except Exception as e:
        logger.error(f"Error decoding base64 image: {str(e)}")
        return None

def mock_donut_recognition(image, document_type="receipt"):
    """
    Mock Donut recognition function
    In a real implementation, this would use the actual Donut model
    """
    try:
        # Simulate processing time and return mock data
        if document_type == "receipt":
            return {
                "extracted_amount": 128.50,
                "extracted_date": "2025-01-19T10:30:00Z",
                "extracted_description": "Restaurant meal",
                "extracted_items": [
                    "Beef noodles - $45.00",
                    "Iced tea - $8.50", 
                    "Service charge - $5.00",
                    "Tax - $10.00"
                ],
                "merchant": "Golden Dragon Restaurant",
                "confidence": 0.92,
                "raw_data": json.dumps({
                    "total": 128.50,
                    "items": ["Beef noodles", "Iced tea", "Service charge", "Tax"],
                    "merchant": "Golden Dragon Restaurant",
                    "date": "2025-01-19"
                })
            }
        elif document_type == "payment":
            return {
                "extracted_amount": 128.50,
                "extracted_date": "2025-01-19T10:35:00Z",
                "payment_method": "WeChat Pay",
                "payer": "John Doe",
                "receiver": "Golden Dragon Restaurant",
                "confidence": 0.89,
                "raw_data": json.dumps({
                    "amount": 128.50,
                    "method": "WeChat Pay",
                    "payer": "John Doe",
                    "receiver": "Golden Dragon Restaurant",
                    "transaction_id": "WX20250119103500123456"
                })
            }
        else:
            return {
                "extracted_amount": None,
                "extracted_date": None,
                "extracted_description": "Unknown document type",
                "confidence": 0.0,
                "raw_data": json.dumps({"error": "Unsupported document type"})
            }
    except Exception as e:
        logger.error(f"Error in mock recognition: {str(e)}")
        return {
            "extracted_amount": None,
            "extracted_date": None,
            "extracted_description": "Recognition failed",
            "confidence": 0.0,
            "raw_data": json.dumps({"error": str(e)})
        }

@donut_bp.route('/recognize/receipt', methods=['POST'])
@cross_origin()
def recognize_receipt():
    """Receipt recognition endpoint"""
    try:
        data = request.get_json()
        
        if not data or 'image' not in data:
            return jsonify({
                "success": False,
                "error": "No image data provided"
            }), 400
        
        # Decode image
        image = decode_base64_image(data['image'])
        if image is None:
            return jsonify({
                "success": False,
                "error": "Invalid image data"
            }), 400
        
        # Perform recognition
        result = mock_donut_recognition(image, "receipt")
        
        return jsonify({
            "success": True,
            "extracted_data": result
        })
        
    except Exception as e:
        logger.error(f"Error in receipt recognition: {str(e)}")
        return jsonify({
            "success": False,
            "error": str(e)
        }), 500

@donut_bp.route('/recognize/payment', methods=['POST'])
@cross_origin()
def recognize_payment():
    """Payment record recognition endpoint"""
    try:
        data = request.get_json()
        
        if not data or 'image' not in data:
            return jsonify({
                "success": False,
                "error": "No image data provided"
            }), 400
        
        # Decode image
        image = decode_base64_image(data['image'])
        if image is None:
            return jsonify({
                "success": False,
                "error": "Invalid image data"
            }), 400
        
        # Perform recognition
        result = mock_donut_recognition(image, "payment")
        
        return jsonify({
            "success": True,
            "extracted_data": result
        })
        
    except Exception as e:
        logger.error(f"Error in payment recognition: {str(e)}")
        return jsonify({
            "success": False,
            "error": str(e)
        }), 500

@donut_bp.route('/recognize/document', methods=['POST'])
@cross_origin()
def recognize_document():
    """Generic document recognition endpoint"""
    try:
        data = request.get_json()
        
        if not data or 'image' not in data:
            return jsonify({
                "success": False,
                "error": "No image data provided"
            }), 400
        
        document_type = data.get('document_type', 'receipt')
        
        # Decode image
        image = decode_base64_image(data['image'])
        if image is None:
            return jsonify({
                "success": False,
                "error": "Invalid image data"
            }), 400
        
        # Perform recognition
        result = mock_donut_recognition(image, document_type)
        
        return jsonify({
            "success": True,
            "extracted_data": result
        })
        
    except Exception as e:
        logger.error(f"Error in document recognition: {str(e)}")
        return jsonify({
            "success": False,
            "error": str(e)
        }), 500

@donut_bp.route('/health', methods=['GET'])
@cross_origin()
def health_check():
    """Health check endpoint"""
    return jsonify({
        "status": "healthy",
        "service": "donut-recognition",
        "version": "1.0.0"
    })

@donut_bp.route('/config', methods=['GET'])
@cross_origin()
def get_config():
    """Get service configuration"""
    return jsonify({
        "supported_formats": ["jpg", "jpeg", "png", "gif", "bmp"],
        "max_image_size": "10MB",
        "supported_document_types": ["receipt", "payment"],
        "model_version": "mock-1.0.0"
    })

