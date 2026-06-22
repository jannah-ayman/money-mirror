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

    if role == "child":
        return {
            "response": child_chatbot_reply(
                message=data.get("message"),
                history=data.get("history", []),
                age=data.get("age"),
                personality_type=data.get("personality_type"),
                personality_child_name=data.get("personality_child_name"),
                current_balance=data.get("current_balance"),
                allowance_amount=data.get("allowance_amount"),
                allowance_type=data.get("allowance_type"),
                top_spending_category=data.get("top_spending_category"),
                top_mood_when_spending=data.get("top_mood_when_spending"),
                total_spent_last_30_days=data.get("total_spent_last_30_days"),
                active_goal_title=data.get("active_goal_title"),
                active_goal_progress_percent=data.get("active_goal_progress_percent"),
                quiz_count=data.get("quiz_count")
            )
        }

    elif role == "parent":
        return {
            "response": parent_chatbot_reply(
                message=data.get("message"),
                history=data.get("history", []), 
                parent_first_name=data.get("parent_first_name"),
                child_first_name=data.get("child_first_name"),
                child_age=data.get("child_age"),
                personality_parent_name=data.get("personality_parent_name"),
                personality_type=data.get("personality_type"),
                traits=data.get("traits"),
                static_recommendations=data.get("static_recommendations"),
                behavioral_dimensions=data.get("behavioral_dimensions"),
                recent_activity=data.get("recent_activity"),
                alerts=data.get("alerts"),
                strengths=data.get("strengths")
            )
        }

    return {"response": "Invalid role. Please specify child or parent."}