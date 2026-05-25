def update_quiz_scores(
    current_scores: dict,
    selected_answer: dict
) -> dict:
    """
    Update personality scores
    based on selected quiz answer.
    """

    updated_scores = current_scores.copy()

    personality = selected_answer.get(
        "personality"
    )

    weight = selected_answer.get(
        "weight",
        1
    )

    # Safety check
    if personality not in updated_scores:
        return updated_scores

    updated_scores[personality] += weight

    return updated_scores