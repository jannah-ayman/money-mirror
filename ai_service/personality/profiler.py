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


def profile_child(features: dict) -> dict:
    scores = calculate_personality_score(features)
    personality_key = max(scores, key=scores.get)

    info = PERSONALITY_MAP[personality_key]

    return {
        "personality_key": personality_key,
        "parent_view": {
            "label": info["parent_name"],
            "description": "Initial financial personality profiling based on behavioral rules."
        },
        "child_view": {
            "label": info["child_name"],
            "message": info["child_message"]
        }
    }
