from ai_service.personality.quiz_scoring import (
    update_quiz_scores
)


current_scores = {
    "IMPULSIVE_SPENDER": 3,
    "PRUDENT_SAVER": 4,
    "GOAL_ORIENTED_PLANNER": 2,
    "BARGAIN_HUNTER": 1
}


selected_answer = {
    "personality": "GOAL_ORIENTED_PLANNER",
    "weight": 3
}


updated_scores = update_quiz_scores(
    current_scores,
    selected_answer
)

print(updated_scores)