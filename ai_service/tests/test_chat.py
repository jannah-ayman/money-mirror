# import requests

# url = "http://127.0.0.1:5000/api/chat"

# data = {
#     "message": "what should i do for my spendings",
#     "personality_type": "IMPULSIVE_SPENDER",
#     "age": 10,
#     "savings_goal": "car"
# }

# response = requests.post(url, json=data)

# print(response.json())




from chatbot.chatbot_service import handle_chatbot_request

# # CHILD TEST
# result = handle_chatbot_request({
#     "role": "child",
#     "message": "when is the world cup?",
#     "age": 12,
#     "personality_type": "GOAL_ORIENTED_PLANNER",
#     "savings_goal": "bike"
# })

# print("\nCHILD RESPONSE:\n", result["response"])


# PARENT TEST
result = handle_chatbot_request({
    "role": "parent",
    "message": "when gonna shakira drop her new album?",
    "parent_name": "Sara",
    "child_name": "Ali",
    "personality_type": "IMPULSIVE_SPENDER",
    "traits": "impulsive",
    "recommendations": "weekly budget",
    "alerts": "high spending",
    "strengths": "sometimes saves",
    "recent_activity": "snacks daily"
})

print("\nPARENT RESPONSE:\n", result["response"])


