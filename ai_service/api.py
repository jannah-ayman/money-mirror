from flask import Flask, request, jsonify
from personality.profiler import profile_child
import logging

app = Flask(__name__)

# Set up logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


@app.route('/health', methods=['GET'])
def health_check():
    """Simple health check endpoint"""
    return jsonify({"status": "healthy", "service": "AI Personality Service"}), 200


@app.route('/api/personality/calculate', methods=['POST'])
def calculate_personality():
    """
    Calculate personality type from questionnaire answers.
    
    Expected JSON body:
    {
        "spending_pace": "Spends it gradually",
        "tries_to_save": "Yes",
        "out_of_money_behavior": "Postpone purchases",
        "reaction_to_100": "Spend part, save part",
        "money_mindset": "Balances spending and saving",
        "spending_categories": ["Food & drinks", "Entertainment"],
        "feeling_after_spending": "Happy",
        "feeling_when_saving_grows": "Motivated"
    }
    
    Returns:
    {
        "success": true,
        "personality_key": "GOAL_ORIENTED_PLANNER",
        "parent_name": "Goal-Oriented Planner",
        "child_name": "Dream Builder",
        "message": "You save money to achieve your dreams!"
    }
    """
    try:
        # Get the questionnaire data from request
        data = request.get_json()
        
        if not data:
            return jsonify({
                "success": False,
                "error": "No data provided"
            }), 400
        
        logger.info(f"Received personality calculation request: {data}")
        
        # Call the AI profiler
        result = profile_child(data)
        
        logger.info(f"Calculated personality: {result['personality_key']}")
        
        # Return the result
        return jsonify({
            "success": True,
            "personality_key": result["personality_key"],
            "parent_name": result["parent_view"]["label"],
            "child_name": result["child_view"]["label"],
            "message": result["child_view"]["message"]
        }), 200
        
    except Exception as e:
        logger.error(f"Error calculating personality: {str(e)}")
        return jsonify({
            "success": False,
            "error": str(e)
        }), 500


if __name__ == '__main__':
    # Run the Flask app
    # In production, use a proper WSGI server like gunicorn
    app.run(host='0.0.0.0', port=5000, debug=True)