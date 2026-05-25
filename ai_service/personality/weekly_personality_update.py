from .behavior_update import (
    update_behavior_scores
)

from .quiz_scoring import (
    update_quiz_scores
)


DEFAULT_SCORES = {
    "IMPULSIVE_SPENDER": 0,
    "PRUDENT_SAVER": 0,
    "GOAL_ORIENTED_PLANNER": 0,
    "BARGAIN_HUNTER": 0
}


def apply_time_weight(
    scores: dict,
    weight: float
) -> dict:
    """
    Apply recency weight to scores.
    Recent behavior matters more.
    """

    return {
        personality: score * weight
        for personality, score in scores.items()
    }


def weekly_personality_update(
    current_scores: dict = None,
    weekly_behavior_data: list = None,
    weekly_quiz_answers: list = None
) -> dict:
    """
    Weekly personality update using:
    - Spending behavior
    - Savings/goals behavior
    - Quiz answers

    More recent data has higher weight.
    """

    # Safe default if no scores exist yet
    updated_scores = (
        current_scores.copy()
        if current_scores
        else DEFAULT_SCORES.copy()
    )

    # Safe fallback
    weekly_behavior_data = (
        weekly_behavior_data or []
    )

    weekly_quiz_answers = (
        weekly_quiz_answers or []
    )

    # ---------------------------------
    # Behavior update
    # ---------------------------------

    total_weeks = max(
        len(weekly_behavior_data),
        1
    )

    for index, behavior_data in enumerate(
        weekly_behavior_data
    ):

        # More recent week = higher weight
        week_weight = (
            (index + 1)
            / total_weeks
        )

        behavior_scores = (
            update_behavior_scores(
                DEFAULT_SCORES.copy(),
                behavior_data
            )
        )

        weighted_behavior = (
            apply_time_weight(
                behavior_scores,
                week_weight
            )
        )

        for personality in updated_scores:

            updated_scores[
                personality
            ] += weighted_behavior.get(
                personality,
                0
            )

    # ---------------------------------
    # Quiz update
    # ---------------------------------

    total_quizzes = max(
        len(weekly_quiz_answers),
        1
    )

    for index, answer in enumerate(
        weekly_quiz_answers
    ):

        # Recent quizzes matter more
        quiz_weight = (
            (index + 1)
            / total_quizzes
        )

        personality = answer.get(
            "personality"
        )

        answer_weight = answer.get(
            "weight",
            1
        )

        if personality in updated_scores:

            updated_scores[
                personality
            ] += (
                answer_weight
                * quiz_weight
            )

    return updated_scores