def calculate_personality_score(features: dict) -> dict:
    scores = {
        "IMPULSIVE_SPENDER": 0,
        "PRUDENT_SAVER": 0,
        "GOAL_ORIENTED_PLANNER": 0,
        "BARGAIN_HUNTER": 0
    }

    # Spending pace
    pace = features.get("spending_pace")
    if pace == "Spends it right away":
        scores["IMPULSIVE_SPENDER"] += 3
    elif pace == "Spends it gradually":
        scores["GOAL_ORIENTED_PLANNER"] += 2
    elif pace == "Saves part of it":
        scores["PRUDENT_SAVER"] += 3

    # Tries to save
    if features.get("tries_to_save") == "Yes":
        scores["PRUDENT_SAVER"] += 2
        scores["GOAL_ORIENTED_PLANNER"] += 1
    else:
        scores["IMPULSIVE_SPENDER"] += 2

    # Out of money behavior
    behavior = features.get("out_of_money_behavior")
    if behavior == "Ask for more allowance":
        scores["IMPULSIVE_SPENDER"] += 2
    elif behavior == "Postpone purchases":
        scores["GOAL_ORIENTED_PLANNER"] += 2
    elif behavior == "Stop spending":
        scores["PRUDENT_SAVER"] += 2

    # Reaction to 100
    reaction = features.get("reaction_to_100")
    if reaction == "Spend it all now on something fun":
        scores["IMPULSIVE_SPENDER"] += 3
    elif reaction == "Spend part, save part":
        scores["GOAL_ORIENTED_PLANNER"] += 2
    elif reaction == "Save it all for a future goal":
        scores["PRUDENT_SAVER"] += 3

    # Money mindset
    mindset = features.get("money_mindset")
    if mindset == "Enjoys spending immediately":
        scores["IMPULSIVE_SPENDER"] += 2
    elif mindset == "Balances spending and saving":
        scores["GOAL_ORIENTED_PLANNER"] += 2
    elif mindset == "Saves for the future":
        scores["PRUDENT_SAVER"] += 2

    # Bargain hunter (weak by design – initial profiling)
    categories = features.get("spending_categories", [])
    if "Clothes & accessories" in categories:
        scores["BARGAIN_HUNTER"] += 1

    # Feelings after spending
    if features.get("feeling_after_spending") == "Regretful":
        scores["PRUDENT_SAVER"] += 1
        scores["GOAL_ORIENTED_PLANNER"] += 1
    elif features.get("feeling_after_spending") == "Happy":
        scores["IMPULSIVE_SPENDER"] += 1

    # Saving emotion
    if features.get("feeling_when_saving_grows") in ["Motivated", "Proud"]:
        scores["PRUDENT_SAVER"] += 2
        scores["GOAL_ORIENTED_PLANNER"] += 1

    return scores
