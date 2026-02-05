from api import app

if __name__ == '__main__':
    print("🚀 Starting AI Personality Service on http://localhost:5000")
    app.run(host='0.0.0.0', port=5000, debug=True)