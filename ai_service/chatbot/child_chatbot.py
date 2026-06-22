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


def child_chatbot_reply(message, personality_type, age, savings_goal):

    # -------------------------
    # 1. Safety filter (STRICT)
    # -------------------------
    if not is_safe_child_message(message):
        return blocked_child_response()

    # -------------------------
    # 2. Domain handling (SOFT REDIRECT instead of blocking)
    # -------------------------
    if not is_money_mirror_related(message):
        message = (
            "The child asked something outside the money topic. "
            "Acknowledge briefly, then redirect the answer to saving, spending, "
            "or financial habits in a friendly way. "
            f"Original message: {message}"
        )

    # -------------------------
    # 3. Build prompt
    # -------------------------
    prompt = build_child_prompt(
        message,
        personality_type,
        age,
        savings_goal
    )

    # -------------------------
    # 4. Get AI response
    # -------------------------
    response = get_response(prompt)

    # -------------------------
    # 5. Safety fallback (ONLY if AI fails)
    # -------------------------
    if not response or len(response.strip()) < 10:
        return blocked_child_response()

    return response