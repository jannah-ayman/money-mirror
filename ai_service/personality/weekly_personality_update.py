from .behavior_update import update_behavior_scores
from .profiler import PERSONALITY_MAP, PERSONALITY_PRIORITY

DEFAULT_SCORES = {
    "IMPULSIVE_SPENDER": 0.0,
    "PRUDENT_SAVER": 0.0,
    "GOAL_ORIENTED_PLANNER": 0.0,
    "BARGAIN_HUNTER": 0.0
}

BEHAVIOR_WEIGHT = 0.7
QUIZ_WEIGHT = 0.3


def normalize_scores(scores: dict) -> dict:
    total = sum(scores.values())

    if total <= 0:
        return {
            k: 25.0
            for k in scores
        }

    return {
        k: round((v / total) * 100, 1)
        for k, v in scores.items()
    }


def calculate_deltas(
    new_scores: dict,
    prev_scores: dict
) -> dict:

    return {
        p: new_scores.get(p, 0)
        - prev_scores.get(p, 0)
        for p in new_scores
    }


def get_winner_personality(
    final_percentages: dict,
    new_raw_scores: dict,
    prev_scores: dict = None
) -> str:

    max_score = max(
        final_percentages.values()
    )

    tied = [
        p
        for p, s in final_percentages.items()
        if abs(s - max_score) < 0.1
    ]

    if len(tied) == 1:
        return tied[0]

    if prev_scores:

        deltas = calculate_deltas(
            new_raw_scores,
            prev_scores
        )

        max_delta = max(
            deltas[p]
            for p in tied
        )

        best = [
            p
            for p in tied
            if abs(
                deltas[p] - max_delta
            ) < 0.1
        ]

        if len(best) == 1:
            return best[0]

        tied = best

    for personality in PERSONALITY_PRIORITY:
        if personality in tied:
            return personality

    return tied[0]


def weekly_personality_update(
    payload: dict
) -> dict:

    raw_prev = payload.get(
        "previous_scores",
        {}
    )

    previous_scores = {
        "IMPULSIVE_SPENDER":
            float(
                raw_prev.get(
                    "impulsive_spender",
                    0.0
                )
            ),

        "PRUDENT_SAVER":
            float(
                raw_prev.get(
                    "prudent_saver",
                    0.0
                )
            ),

        "GOAL_ORIENTED_PLANNER":
            float(
                raw_prev.get(
                    "goal_oriented_planner",
                    0.0
                )
            ),

        "BARGAIN_HUNTER":
            float(
                raw_prev.get(
                    "bargain_hunter",
                    0.0
                )
            )
    }

    updated_scores = previous_scores.copy()

    # --------------------------
    # Behavior Update (70%)
    # --------------------------

    behavior_scores = update_behavior_scores(
        DEFAULT_SCORES.copy(),
        payload
    )

    for personality in updated_scores:

        updated_scores[
            personality
        ] += (
            behavior_scores.get(
                personality,
                0
            )
            * BEHAVIOR_WEIGHT
        )

    # --------------------------
    # Quiz Update (30%)
    # Count + Recency Weighting
    # --------------------------

    quiz_answers = payload.get(
        "quiz_answers",
        []
    )

    total_answers = len(
        quiz_answers
    )

    if total_answers > 0:

        quiz_scores = {
            "IMPULSIVE_SPENDER": 0.0,
            "PRUDENT_SAVER": 0.0,
            "GOAL_ORIENTED_PLANNER": 0.0,
            "BARGAIN_HUNTER": 0.0
        }

        for index, answer in enumerate(
            quiz_answers
        ):

            personality = answer.get(
                "personality"
            )

            if personality not in quiz_scores:
                continue

            # newer answers matter more
            recency_weight = (
                index + 1
            ) / total_answers

            quiz_scores[
                personality
            ] += recency_weight

        for personality in updated_scores:

            updated_scores[
                personality
            ] += (
                quiz_scores.get(
                    personality,
                    0
                )
                * QUIZ_WEIGHT
            )

    final_scores = normalize_scores(
        updated_scores
    )

    personality_key = (
        get_winner_personality(
            final_scores,
            updated_scores,
            previous_scores
        )
    )

    info = PERSONALITY_MAP[
        personality_key
    ]

    return {
        "success": True,
        "personality_key":
            personality_key,

        "parent_name":
            info["parent_name"],

        "scores":
            updated_scores,

        "dimensions":
            final_scores
    }