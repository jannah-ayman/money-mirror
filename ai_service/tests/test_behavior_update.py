from ai_service.personality.behavior_update import (
    update_behavior_scores
)


current_scores = {
    "IMPULSIVE_SPENDER": 3,
    "PRUDENT_SAVER": 2,
    "GOAL_ORIENTED_PLANNER": 4,
    "BARGAIN_HUNTER": 1
}


behavior_data = {
    "spending_frequency": 12,
    "savings_ratio": 0.6,
    "regretful_moods": 2,
    "happy_spending_moods": 6,
    "goals_completed": 3,
    "discount_usage": 4
}


updated_scores = update_behavior_scores(
    current_scores,
    behavior_data
)

print(updated_scores)