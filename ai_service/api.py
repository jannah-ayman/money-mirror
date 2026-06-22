# from flask import Flask, request, jsonify
# from personality.profiler import profile_child
# from chatbot.chatbot_service import chatbot_reply
# import logging

# app = Flask(__name__)

# # Set up logging
# logging.basicConfig(level=logging.INFO)
# logger = logging.getLogger(__name__)


# @app.route('/health', methods=['GET'])
# def health_check():
#     """Simple health check endpoint"""
#     return jsonify({"status": "healthy", "service": "AI Personality Service"}), 200


# @app.route('/api/personality/calculate', methods=['POST'])
# def calculate_personality():
#     """
#     Calculate personality type from questionnaire answers.
    
#     Expected JSON body:
#     {
#         "spending_pace": "Spends it gradually",
#         "tries_to_save": "Yes",
#         "out_of_money_behavior": "Postpone purchases",
#         "reaction_to_100": "Spend part, save part",
#         "money_mindset": "Balances spending and saving",
#         "spending_categories": ["Food & drinks", "Entertainment"],
#         "feeling_after_spending": "Happy",
#         "feeling_when_saving_grows": "Motivated"
#     }
    
#     Returns:
#     {
#         "success": true,
#         "personality_key": "GOAL_ORIENTED_PLANNER",
#         "parent_name": "Goal-Oriented Planner",
#         "child_name": "Dream Builder",
#         "message": "You save money to achieve your dreams!"
#     }
#     """
#     try:
#         # Get the questionnaire data from request
#         data = request.get_json()
        
#         if not data:
#             return jsonify({
#                 "success": False,
#                 "error": "No data provided"
#             }), 400
        
#         logger.info(f"Received personality calculation request: {data}")
        
#         # Call the AI profiler
#         result = profile_child(data)
        
#         logger.info(f"Calculated personality: {result['personality_key']}")
        
#         # Return the result
#         return jsonify({
#             "success": True,
#             "personality_key": result["personality_key"],
#             "parent_name": result["parent_view"]["label"],
#             "child_name": result["child_view"]["label"],
#             "message": result["child_view"]["message"]
#         }), 200
        
#     except Exception as e:
#         logger.error(f"Error calculating personality: {str(e)}")
#         return jsonify({
#             "success": False,
#             "error": str(e)
#         }), 500


# @app.route('/api/chat', methods=['POST'])
# def chat():

#     try:
#         data = request.get_json()

#         if not data:
#             return jsonify({
#                 "success": False,
#                 "error": "No data provided"
#             }), 400

#         message = data.get("message")

#         if not message:
#             return jsonify({
#                 "success": False,
#                 "error": "Message is required"
#             }), 400

#         # OPTIONAL FIELDS (safe if missing)
#         personality_type = data.get("personality_type")
#         age = data.get("age")
#         savings_goal = data.get("savings_goal")

#         reply = chatbot_reply(
#             message,
#             personality_type,
#             age,
#             savings_goal
#         )

#         return jsonify({
#             "success": True,
#             "reply": reply
#         }), 200

#     except Exception as e:
#         return jsonify({
#             "success": False,
#             "error": str(e)
#         }), 500
    


    
# if __name__ == '__main__':
#     # Run the Flask app
#     # In production, use a proper WSGI server like gunicorn
#     app.run(host='0.0.0.0', port=5000, debug=True)



















from flask import Flask, request, jsonify
from personality.profiler import profile_child
from chatbot.chatbot_service import handle_chatbot_request
import logging

app = Flask(__name__)

# Logging setup
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


# ==========================================
# HEALTH CHECK
# ==========================================
@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({
        "status": "healthy",
        "service": "AI Personality Service"
    }), 200


# ==========================================
# PERSONALITY CALCULATION
# ==========================================
@app.route('/api/personality/calculate', methods=['POST'])
def calculate_personality():

    try:
        data = request.get_json()

        if not data:
            return jsonify({
                "success": False,
                "error": "No data provided"
            }), 400

        logger.info("Personality request received")

        result = profile_child(data)

        logger.info(
            f"Personality result: {result['personality_key']}"
        )

        return jsonify({
            "success": True,
            "personality_key": result["personality_key"],
            "parent_name": result["parent_view"]["label"],
            "child_name": result["child_view"]["label"],
            "message": result["child_view"]["message"]
        }), 200

    except Exception as e:

        logger.error(f"Personality error: {str(e)}")

        return jsonify({
            "success": False,
            "error": str(e)
        }), 500


# ==========================================
# CHATBOT ENDPOINT
# ==========================================
@app.route('/api/chat', methods=['POST'])
def chat():

    try:

        data = request.get_json()

        if not data:
            return jsonify({
                "success": False,
                "error": "No data provided"
            }), 400

        if not data.get("message"):
            return jsonify({
                "success": False,
                "error": "Message is required"
            }), 400

        if not data.get("role"):
            return jsonify({
                "success": False,
                "error": "Role is required (child or parent)"
            }), 400

        logger.info(
            f"Chat request received: role={data.get('role')}"
        )

        result = handle_chatbot_request(data)

        return jsonify({
            "success": True,
            "response": result["response"]
        }), 200

    except Exception as e:

        logger.error(
            f"Chat error: {str(e)}"
        )

        return jsonify({
            "success": False,
            "error": str(e)
        }), 500


# ==========================================
# RUN SERVER
# ==========================================
if __name__ == "__main__":

    print("🚀 AI Service running on http://127.0.0.1:5000")

    app.run(
        host="0.0.0.0",
        port=5000,
        debug=True
    )

