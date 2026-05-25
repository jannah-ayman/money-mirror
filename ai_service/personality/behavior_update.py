# Future behavior-based personality adjustment logic
def update_behavior_scores(
    current_scores: dict,
    behavior_data: dict
) -> dict:
    """
    Adjust personality scores over time
    based on child financial behavior.
    """

    updated_scores = current_scores.copy()

    
    # Spending frequency
    
    spending_frequency = behavior_data.get(
        "spending_frequency",
        0
    )

    if spending_frequency >= 10:
        updated_scores["IMPULSIVE_SPENDER"] += 2
    elif spending_frequency <= 3:
        updated_scores["PRUDENT_SAVER"] += 1

    
    # Savings progress
    
    savings_ratio = behavior_data.get(
        "savings_ratio",
        0
    )

    if savings_ratio >= 0.5:
        updated_scores["PRUDENT_SAVER"] += 3
        updated_scores["GOAL_ORIENTED_PLANNER"] += 2
    elif savings_ratio < 0.2:
        updated_scores["IMPULSIVE_SPENDER"] += 2

    
    # Mood after spending
    
    regret_count = behavior_data.get(
        "regretful_moods",
        0
    )

    happy_spending = behavior_data.get(
        "happy_spending_moods",
        0
    )

    if regret_count >= 3:
        updated_scores["PRUDENT_SAVER"] += 2

    if happy_spending >= 5:
        updated_scores["IMPULSIVE_SPENDER"] += 2

    
    # Goal tracking
    
    goals_completed = behavior_data.get(
        "goals_completed",
        0
    )

    if goals_completed >= 2:
        updated_scores["GOAL_ORIENTED_PLANNER"] += 3

    
    # Bargain behavior
 
    discount_usage = behavior_data.get(
        "discount_usage",
        0
    )

    if discount_usage >= 3:
        updated_scores["BARGAIN_HUNTER"] += 3

    return updated_scores