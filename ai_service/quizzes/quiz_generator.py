import json

from .prompt_templates import (
    QUIZ_GENERATION_PROMPT
)

from .llm_client import (
    generate_questions_from_llm
)


def validate_question(
    question: dict
) -> bool:
    """
    Validate generated question.
    """

    required_personalities = {
        "IMPULSIVE_SPENDER",
        "PRUDENT_SAVER",
        "GOAL_ORIENTED_PLANNER",
        "BARGAIN_HUNTER"
    }

    # Must contain story
    if (
        "story" not in question
        or not question["story"]
    ):
        return False

    # Must contain category
    if (
        "category" not in question
        or not question["category"]
    ):
        return False

    # Must contain choices
    choices = question.get(
        "choices",
        []
    )

    # Must be exactly 4 choices
    if len(choices) != 4:
        return False

    personalities = []
    choice_texts = []

    for choice in choices:

        # Required fields
        if (
            "text" not in choice
            or "personality" not in choice
            or "weight" not in choice
        ):
            return False

        # No empty text
        if not choice["text"].strip():
            return False

        personalities.append(
            choice["personality"]
        )

        choice_texts.append(
            choice["text"].strip().lower()
        )

    # Must contain all 4 exactly once
    if set(personalities) != required_personalities:
        return False

    # No duplicate personalities
    if len(personalities) != len(
        set(personalities)
    ):
        return False

    # No duplicate answer text
    if len(choice_texts) != len(
        set(choice_texts)
    ):
        return False

    return True


def build_generation_prompt(
    category: str,
    count: int = 10
) -> str:
    """
    Build prompt for LLM.
    """

    return f"""
{QUIZ_GENERATION_PROMPT}

Generate {count} quiz questions.

Category:
{category}

IMPORTANT:
Return VALID JSON ARRAY ONLY.

No markdown.
No explanations.
No extra text.
"""


def parse_generated_questions(
    response_text: str
) -> list:
    """
    Parse LLM JSON response safely.
    """

    try:
        questions = json.loads(
            response_text
        )

        valid_questions = []

        for question in questions:

            if validate_question(
                question
            ):
                valid_questions.append(
                    question
                )

        return valid_questions

    except Exception as e:
        print(
            "JSON Parse Error:",
            e
        )
        return []


def generate_quiz_questions(
    category: str,
    count: int = 10
) -> list:
    """
    Generate quiz questions
    using Claude 3.5 Sonnet.
    """

    prompt = build_generation_prompt(
        category=category,
        count=count
    )

    response = (
        generate_questions_from_llm(
            prompt
        )
    )

    questions = (
        parse_generated_questions(
            response
        )
    )

    return questions