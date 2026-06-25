from .parent_filter import (
    is_safe_parent_message,
    blocked_parent_response
)
from .prompt_builders import build_parent_prompt
from .provider_manager import get_response


def detect_language(text):
    arabic_chars = sum(1 for c in text if '\u0600' <= c <= '\u06FF')
    return "ar" if arabic_chars > len(text) * 0.3 else "en"


def parent_chatbot_reply(
        message,
        history,
        parent_first_name,
        child_first_name,
        child_age,
        personality_parent_name,
        personality_type,
        traits,
        static_recommendations,
        behavioral_dimensions,
        recent_activity,
        alerts,
        strengths):

    if not is_safe_parent_message(message):
        return blocked_parent_response()

    language = detect_language(message)

    prompt = build_parent_prompt(
        message=message,
        history=history,
        parent_first_name=parent_first_name,
        child_first_name=child_first_name,
        child_age=child_age,
        personality_parent_name=personality_parent_name,
        personality_type=personality_type,
        traits=traits,
        static_recommendations=static_recommendations,
        behavioral_dimensions=behavioral_dimensions,
        recent_activity=recent_activity,
        alerts=alerts,
        strengths=strengths,
        language=language
    )

    response = get_response(prompt)

    if not response or len(response.strip()) < 10:
        return blocked_parent_response()

    return response