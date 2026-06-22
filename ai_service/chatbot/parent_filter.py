BLOCKED_WORDS = [
    "porn",
    "bomb",
    "weapon"
]

# -----------------------------
# SAFETY CHECK (STRICT ONLY)
# -----------------------------
def is_safe_parent_message(message):
    message = message.lower()
    return not any(word in message for word in BLOCKED_WORDS)


# -----------------------------
# RESPONSE WHEN BLOCKED
# -----------------------------
def blocked_parent_response():
    return (
        "I can’t help with that request. "
        "I’m here to support you in understanding and improving "
        "your child’s financial habits."
    )