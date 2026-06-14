def format_question(
    question_id: int,
    category: str,
    story: str,
    choices: list
) -> dict:
    """
    Format quiz question
    into standard structure.
    """

    return {
        "question_id": question_id,
        "category": category,
        "story": story,
        "choices": choices
    }