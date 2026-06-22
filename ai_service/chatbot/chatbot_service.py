# from chatbot.provider_manager import get_response


# def chatbot_reply(
#         message,
#         personality_type=None,
#         age=None,
#         savings_goal=None
# ):

#     personality_map = {
#         "IMPULSIVE_SPENDER": "Speedy Spender",
#         "GOAL_ORIENTED_PLANNER": "Dream Builder",
#         "PRUDENT_SAVER": "Wise Saver",
#         "BARGAIN_HUNTER": "Deal Hunter"
#     }

#     child_type = personality_map.get(
#         personality_type,
#         "Money Explorer"
#     )

#     prompt = f"""
# You are Money Mirror, a friendly financial assistant for children.

# Child Profile:
# - Personality: {child_type}
# - Age: {age if age else "Unknown"}
# - Savings Goal: {savings_goal if savings_goal else "Not set"}

# User Message:
# {message}

# Rules:
# - Always respond in a friendly, encouraging tone
# - Use simple language for children (age 6–14)
# - Adapt advice based on personality:
#   - Speedy Spender → teach delay before spending
#   - Wise Saver → reinforce saving habits
#   - Dream Builder → motivate toward goals
#   - Deal Hunter → teach smart shopping choices
# - Keep response short (3–6 lines max)
# """

#     return get_response(prompt)
























from .child_chatbot import child_chatbot_reply
from .parent_chatbot import parent_chatbot_reply


def handle_chatbot_request(data):

    role = data.get("role")

    # CHILD MODE
    if role == "child":

        return {
            "response": child_chatbot_reply(
                message=data.get("message"),
                personality_type=data.get("personality_type"),
                age=data.get("age"),
                savings_goal=data.get("savings_goal") or data.get("goal")
            )
        }

    # PARENT MODE
    elif role == "parent":

        return {
            "response": parent_chatbot_reply(
                message=data.get("message"),
                parent_name=data.get("parent_name"),
                child_name=data.get("child_name"),
                personality_type=data.get("personality_type"),
                traits=data.get("traits"),
                recommendations=data.get("recommendations"),
                alerts=data.get("alerts"),
                strengths=data.get("strengths"),
                recent_activity=data.get("recent_activity")
            )
        }

    return {"response": "Invalid role. Please specify child or parent."}