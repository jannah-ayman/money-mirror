def update_behavior_scores(
    current_scores: dict,
    behavior_data: dict
) -> dict:
    """
    Update personality scores
    using monthly behavioral data.
    """

    updated_scores = current_scores.copy()

    allowance_type = behavior_data.get(
        "allowance_type",
        ""
    )

    spending_frequency = behavior_data.get(
        "spending_frequency",
        0
    )

    total_spend = behavior_data.get(
        "total_spend",
        0.0
    )

    average_spend = behavior_data.get(
        "average_spend_per_transaction",
        0.0
    )

    savings_ratio = behavior_data.get(
        "savings_ratio",
        0.0
    )

    goal_completion_rate = behavior_data.get(
        "goal_completion_rate",
        0.0
    )

    mood_spending = behavior_data.get(
        "mood_spending",
        {}
    )

    top_categories = behavior_data.get(
        "top_categories",
        []
    )

    # ----------------------------------
    # Spending frequency
    # ----------------------------------

    if allowance_type == "Monthly":

        if spending_frequency >= 12:
            updated_scores[
                "IMPULSIVE_SPENDER"
            ] += 4

        elif spending_frequency <= 4:
            updated_scores[
                "PRUDENT_SAVER"
            ] += 2

    elif allowance_type == "Weekly":

        if spending_frequency >= 16:
            updated_scores[
                "IMPULSIVE_SPENDER"
            ] += 3

        elif spending_frequency <= 6:
            updated_scores[
                "PRUDENT_SAVER"
            ] += 2

    elif allowance_type == "Daily":

        if spending_frequency >= 25:
            updated_scores[
                "IMPULSIVE_SPENDER"
            ] += 2

    # ----------------------------------
    # Average transaction value
    # ----------------------------------

    if average_spend >= 100:
        updated_scores[
            "IMPULSIVE_SPENDER"
        ] += 2

    elif average_spend <= 20:
        updated_scores[
            "PRUDENT_SAVER"
        ] += 1

    # ----------------------------------
    # Savings ratio
    # ----------------------------------

    if savings_ratio >= 0.70:

        updated_scores[
            "PRUDENT_SAVER"
        ] += 5

        updated_scores[
            "GOAL_ORIENTED_PLANNER"
        ] += 3

    elif savings_ratio >= 0.40:

        updated_scores[
            "PRUDENT_SAVER"
        ] += 3

        updated_scores[
            "GOAL_ORIENTED_PLANNER"
        ] += 2

    elif savings_ratio <= 0.15:

        updated_scores[
            "IMPULSIVE_SPENDER"
        ] += 4

    # ----------------------------------
    # Goal completion
    # ----------------------------------

    if goal_completion_rate >= 0.80:

        updated_scores[
            "GOAL_ORIENTED_PLANNER"
        ] += 5

    elif goal_completion_rate >= 0.50:

        updated_scores[
            "GOAL_ORIENTED_PLANNER"
        ] += 3

    # ----------------------------------
    # Emotional spending
    # ----------------------------------

    happy_ratio = mood_spending.get(
        "Happy",
        0
    )

    excited_ratio = mood_spending.get(
        "Excited",
        0
    )

    if (
        happy_ratio +
        excited_ratio
    ) >= 0.70:

        updated_scores[
            "IMPULSIVE_SPENDER"
        ] += 3

    # ----------------------------------
    # Categories
    # ----------------------------------

    bargain_categories = {
        "Clothes / Fashion",
        "Shopping",
        "Accessories"
    }

    planner_categories = {
        "Education",
        "Books",
        "Courses"
    }

    if any(
        category in bargain_categories
        for category in top_categories
    ):
        updated_scores[
            "BARGAIN_HUNTER"
        ] += 2

    if any(
        category in planner_categories
        for category in top_categories
    ):
        updated_scores[
            "GOAL_ORIENTED_PLANNER"
        ] += 2

    # ----------------------------------
    # High spend + low saving
    # ----------------------------------

    if (
        total_spend > 0
        and spending_frequency > 0
        and savings_ratio < 0.15
    ):
        updated_scores[
            "IMPULSIVE_SPENDER"
        ] += 2

    return updated_scores