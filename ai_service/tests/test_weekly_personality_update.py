from ai_service.personality.weekly_personality_update import (
    weekly_personality_update
)


current_scores = {
    "IMPULSIVE_SPENDER": 2,
    "PRUDENT_SAVER": 2,
    "GOAL_ORIENTED_PLANNER": 2,
    "BARGAIN_HUNTER": 2
}


weekly_behavior_data = [
    {
        "spending_frequency": 10,
        "savings_ratio": 0.1,
        "regretful_moods": 1,
        "happy_spending_moods": 5,
        "goals_completed": 0,
        "discount_usage": 0
    },
    {
        "spending_frequency": 3,
        "savings_ratio": 0.6,
        "regretful_moods": 4,
        "happy_spending_moods": 1,
        "goals_completed": 2,
        "discount_usage": 3
    }
]


weekly_quiz_answers = [
    {
        "personality": "IMPULSIVE_SPENDER",
        "weight": 2
    },
    {
        "personality": "GOAL_ORIENTED_PLANNER",
        "weight": 3
    }
]


result = weekly_personality_update(
    current_scores,
    weekly_behavior_data,
    weekly_quiz_answers
)

print(result)