from collections import defaultdict


PERSONALITIES = [
    "IMPULSIVE_SPENDER",
    "PRUDENT_SAVER",
    "GOAL_ORIENTED_PLANNER",
    "BARGAIN_HUNTER"
]


def initialize_scores() -> dict:
    """
    Create empty score dictionary.
    """

    return {
        personality: 0.0
        for personality in PERSONALITIES
    }


def calculate_quiz_scores(
    quiz_answers: list
) -> dict:
    """
    Calculate quiz personality scores
    using answer counts with recency weighting.

    More recent answers
    have stronger influence.
    """

    scores = initialize_scores()

    if not quiz_answers:
        return scores

    total_answers = len(quiz_answers)

    for index, answer in enumerate(
        quiz_answers
    ):

        personality = answer.get(
            "personality"
        )

        if personality not in scores:
            continue

        # Recency weight
        recency_weight = (
            (index + 1)
            / total_answers
        )

        scores[
            personality
        ] += recency_weight

    return scores


def get_quiz_counts(
    quiz_answers: list
) -> dict:
    """
    Return raw answer counts
    per personality.
    """

    counts = {
        personality: 0
        for personality in PERSONALITIES
    }

    for answer in quiz_answers:

        personality = answer.get(
            "personality"
        )

        if personality in counts:
            counts[
                personality
            ] += 1

    return counts


def get_top_quiz_personality(
    quiz_answers: list
) -> str | None:
    """
    Return dominant personality
    from quiz answers only.
    """

    scores = calculate_quiz_scores(
        quiz_answers
    )

    max_score = max(
        scores.values(),
        default=0
    )

    if max_score == 0:
        return None

    winners = [
        personality
        for personality, score
        in scores.items()
        if score == max_score
    ]

    return winners[0]