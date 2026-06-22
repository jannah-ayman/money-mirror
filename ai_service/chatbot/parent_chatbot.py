from .parent_filter import (
    is_safe_parent_message,
    blocked_parent_response
)

from .prompt_builders import build_parent_prompt
from .provider_manager import get_response


def parent_chatbot_reply(
    message,
    parent_name,
    child_name,
    personality_type,
    traits,
    recommendations,
    alerts,
    strengths,
    recent_activity
):

    # 1. Safety filter
    if not is_safe_parent_message(message):
        return blocked_parent_response()

    # 2. Build prompt
    prompt = build_parent_prompt(
        message,
        parent_name,
        child_name,
        personality_type,
        traits,
        recommendations,
        alerts,
        strengths,
        recent_activity
    )

    # 3. Get AI response
    response = get_response(prompt)

    # 4. Output safety check
    if not response or len(response.strip()) < 10:
        return blocked_parent_response()

    return response