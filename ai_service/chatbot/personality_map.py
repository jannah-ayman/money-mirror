PERSONALITY_MAP = {
    "IMPULSIVE_SPENDER": "Speedy Spender",
    "GOAL_ORIENTED_PLANNER": "Dream Builder",
    "PRUDENT_SAVER": "Wise Saver",
    "BARGAIN_HUNTER": "Deal Hunter"
}


def get_child_personality_label(personality_type):
    return PERSONALITY_MAP.get(personality_type, "Money Explorer")