# from .child_filter import (
#     is_safe_child_message,
#     is_money_mirror_related,
#     blocked_child_response,
#     unrelated_question_response
# )

# from .prompt_builders import build_child_prompt
# from .provider_manager import get_response


# def child_chatbot_reply(message, personality_type, age, savings_goal):

#     # 1. Safety filter
#     if not is_safe_child_message(message):
#         return blocked_child_response()

#     # 2. Domain filter (Money Mirror only)
#     if not is_money_mirror_related(message):
#         return unrelated_question_response()

#     # 3. Build prompt
#     prompt = build_child_prompt(
#         message,
#         personality_type,
#         age,
#         savings_goal
#     )

#     # 4. Get AI response
#     response = get_response(prompt)

#     # 5. Safety check on output
#     if not response or len(response.strip()) < 10:
#         return blocked_child_response()

#     return response



from .child_filter import (
    is_safe_child_message,
    is_money_mirror_related,
    blocked_child_response,
    unrelated_question_response
)

from .prompt_builders import build_child_prompt
from .provider_manager import get_response
def child_chatbot_reply(
        message,
        history,
        age,
        personality_type,
        personality_child_name,
        current_balance,
        allowance_amount,
        allowance_type,
        top_spending_category,
        top_mood_when_spending,
        total_spent_last_30_days,
        active_goal_title,
        active_goal_progress_percent,
        quiz_count):

    if not is_safe_child_message(message):
        return blocked_child_response()

    if not is_money_mirror_related(message):
        message = (
            "The child asked something outside the money topic. "
            "Acknowledge briefly, then redirect the answer to saving, spending, "
            "or financial habits in a friendly way. "
            f"Original message: {message}"
        )

    prompt = build_child_prompt(
        message=message,
        history=history,
        age=age,
        personality_type=personality_type,
        personality_child_name=personality_child_name,
        current_balance=current_balance,
        allowance_amount=allowance_amount,
        allowance_type=allowance_type,
        top_spending_category=top_spending_category,
        top_mood_when_spending=top_mood_when_spending,
        total_spent_last_30_days=total_spent_last_30_days,
        active_goal_title=active_goal_title,
        active_goal_progress_percent=active_goal_progress_percent,
        quiz_count=quiz_count
    )

    response = get_response(prompt)

    if not response or len(response.strip()) < 10:
        return blocked_child_response()

    return response