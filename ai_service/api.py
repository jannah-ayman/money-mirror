from flask import Flask, request, jsonify
from personality.profiler import profile_child
from personality.weekly_personality_update import weekly_personality_update
import logging

app = Flask(__name__)

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({"status": "healthy", "service": "AI Personality Service"}), 200


@app.route('/api/personality/calculate', methods=['POST'])
def calculate_personality():
    try:
        data = request.get_json()
        if not data:
            return jsonify({"success": False, "error": "No data provided"}), 400

        logger.info(f"Received personality calculation request: {data}")
        result = profile_child(data)
        logger.info(f"Calculated personality: {result['personality_key']}")

        return jsonify({
            "success": True,
            "personality_key": result["personality_key"],
            "parent_name": result["parent_view"]["label"],
            "child_name": result["child_view"]["label"],
            "message": result["child_view"]["message"]
        }), 200

    except Exception as e:
        logger.error(f"Error calculating personality: {str(e)}")
        return jsonify({"success": False, "error": str(e)}), 500


@app.route('/api/personality/weekly-update', methods=['POST'])
def weekly_update():
    try:
        data = request.get_json()
        if not data:
            return jsonify({"success": False, "error": "No data provided"}), 400

        logger.info(f"Received weekly update request for child_id: {data.get('child_id')}")
        result = weekly_personality_update(data)
        logger.info(f"Weekly update result: {result['personality_key']}")

        return jsonify({
            "success": True,
            "personality_key": result["personality_key"],
            "parent_name": result["parent_name"],
            "dimensions": result["dimensions"]
        }), 200

    except Exception as e:
        logger.error(f"Error in weekly update: {str(e)}")
        return jsonify({"success": False, "error": str(e)}), 500


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)