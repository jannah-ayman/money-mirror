from .behavior_update import update_behavior_scores
from .profiler import PERSONALITY_MAP, PERSONALITY_PRIORITY

DEFAULT_SCORES = {
    "IMPULSIVE_SPENDER": 0.0,
    "PRUDENT_SAVER": 0.0,
    "GOAL_ORIENTED_PLANNER": 0.0,
    "BARGAIN_HUNTER": 0.0
}


def normalize_scores(scores: dict) -> dict:
    total = sum(scores.values())
    if total <= 0:
        return {k: 25.0 for k in scores}
    return {k: round((v / total) * 100, 1) for k, v in scores.items()}


def calculate_deltas(new_scores: dict, prev_scores: dict) -> dict:
    return {p: new_scores.get(p, 0) - prev_scores.get(p, 0) for p in new_scores}


def get_winner_personality(final_percentages: dict, new_raw_scores: dict, prev_scores: dict = None) -> str:
    max_score = max(final_percentages.values())
    tied = [p for p, s in final_percentages.items() if abs(s - max_score) < 0.1]

    if len(tied) == 1:
        return tied[0]

    # Trend-based tie-breaking using raw points growth rather than percentage fractions
    if prev_scores:
        deltas = calculate_deltas(new_raw_scores, prev_scores)
        max_delta = max(deltas[p] for p in tied)
        best = [p for p in tied if abs(deltas[p] - max_delta) < 0.1]
        if len(best) == 1:
            return best[0]
        tied = best

    for p in PERSONALITY_PRIORITY:
        if p in tied:
            return p
    return tied[0]


def weekly_personality_update(payload: dict) -> dict:
    # 1. Safely map lowercase C# keys to uppercase Python dictionary constants
    raw_prev = payload.get("previous_scores", {})
    previous_scores = {
        "IMPULSIVE_SPENDER": float(raw_prev.get("impulsive_spender", 0.0)),
        "PRUDENT_SAVER": float(raw_prev.get("prudent_saver", 0.0)),
        "GOAL_ORIENTED_PLANNER": float(raw_prev.get("goal_oriented_planner", 0.0)),
        "BARGAIN_HUNTER": float(raw_prev.get("bargain_hunter", 0.0))
    }
    
    # 2. Extract quizzes and behavioral fields smoothly if sent via unified object structure
    csharp_quiz = payload.get("quiz_scores", {})
    weekly_quiz_answers = payload.get("weekly_quiz_answers", [])
    
    if csharp_quiz and not weekly_quiz_answers:
        weekly_quiz_answers = [
            {"personality": "IMPULSIVE_SPENDER", "weight": csharp_quiz.get("impulsive", 0)},
            {"personality": "PRUDENT_SAVER", "weight": csharp_quiz.get("saver", 0)},
            {"personality": "GOAL_ORIENTED_PLANNER", "weight": csharp_quiz.get("planner", 0)},
            {"personality": "BARGAIN_HUNTER", "weight": csharp_quiz.get("bargain", 0)}
        ]

    weekly_behavior_data = payload.get("weekly_behavior_data", [])
    if not weekly_behavior_data:
        # Gracefully process root fields if it's sent as a single snapshot
        weekly_behavior_data = [payload]

    updated_scores = previous_scores.copy()

    # Behavior updates with recency weighting
    total_weeks = max(len(weekly_behavior_data), 1)
    for index, behavior_data in enumerate(weekly_behavior_data):
        week_weight = (index + 1) / total_weeks
        behavior_scores = update_behavior_scores(DEFAULT_SCORES.copy(), behavior_data)
        
        for personality in updated_scores:
            updated_scores[personality] += behavior_scores.get(personality, 0) * week_weight

    # Quiz updates with recency weighting
    total_quizzes = max(len(weekly_quiz_answers), 1)
    for index, answer in enumerate(weekly_quiz_answers):
        quiz_weight = (index + 1) / total_quizzes
        personality = answer.get("personality")
        answer_weight = answer.get("weight", 1)
        
        if personality in updated_scores:
            updated_scores[personality] += (answer_weight * quiz_weight * 2.0)

    # 3. Create normal percentage values out of aggregated final tracking scores
    final_scores = normalize_scores(updated_scores)
    
    # 4. Solve potential ties correctly passing percentages alongside raw metrics
    personality_key = get_winner_personality(final_scores, updated_scores, previous_scores)

    info = PERSONALITY_MAP[personality_key]

    return {
        "success": True,
        "personality_key": personality_key,
        "parent_name": info["parent_name"],
        "scores": updated_scores,
        "dimensions": final_scores
    }