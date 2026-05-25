from .scoring import calculate_personality_score


PERSONALITY_MAP = {
    "IMPULSIVE_SPENDER": {
        "parent_name": "Impulsive Spender",
        "child_name": "Speedy Spender",
        "child_message": "You love spending money quickly on fun things!"
    },
    "PRUDENT_SAVER": {
        "parent_name": "Prudent Saver",
        "child_name": "Treasure Keeper",
        "child_message": "You are great at saving and protecting your money!"
    },
    "GOAL_ORIENTED_PLANNER": {
        "parent_name": "Goal-Oriented Planner",
        "child_name": "Dream Builder",
        "child_message": "You save money to achieve your dreams!"
    },
    "BARGAIN_HUNTER": {
        "parent_name": "Bargain Hunter",
        "child_name": "Deal Detective",
        "child_message": "You are smart at finding the best deals!"
    }
}


# Priority order in case of tie
PERSONALITY_PRIORITY = [
    "GOAL_ORIENTED_PLANNER",
    "PRUDENT_SAVER",
    "BARGAIN_HUNTER",
    "IMPULSIVE_SPENDER"
]


def profile_child(features: dict) -> dict:
    # Calculate scores
    scores = calculate_personality_score(features)

    # Highest score
    max_score = max(scores.values())

    # Find all personalities with highest score
    tied_personalities = [
        personality
        for personality, score in scores.items()
        if score == max_score
    ]

    # If tie → choose based on priority
    if len(tied_personalities) > 1:
        personality_key = next(
            personality
            for personality in PERSONALITY_PRIORITY
            if personality in tied_personalities
        )
    else:
        personality_key = tied_personalities[0]

    # Get personality info
    info = PERSONALITY_MAP[personality_key]

    return {
        "personality_key": personality_key,
        "scores": scores,  # remove later if not needed
        "parent_view": {
            "label": info["parent_name"],
            "description": (
                "Initial financial personality profiling "
                "based on behavioral rules."
            )
        },
        "child_view": {
            "label": info["child_name"],
            "message": info["child_message"]
        }
    }
