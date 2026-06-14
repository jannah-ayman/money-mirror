QUIZ_GENERATION_PROMPT = """
You are generating financial literacy quiz questions
for children aged 6–14.

Your task:
Generate engaging, realistic, story-based
financial quiz questions.

STRICT RULES:

1. Questions must be child-friendly.

2. Questions must be GENERAL and unbiased.
Do NOT target or favor any specific personality.

3. Questions must feel realistic and fun.
Use daily situations children can relate to:
school, snacks, toys, gifts, birthdays,
friends, shopping, games, saving money,
allowance, family outings, etc.

4. Avoid repeating similar wording
or repetitive scenarios.

5. Each question MUST contain
EXACTLY 4 choices.

6. CRITICAL:
Each choice MUST map to ONE UNIQUE personality.

You MUST use ALL four personalities
EXACTLY ONCE:

- IMPULSIVE_SPENDER
- PRUDENT_SAVER
- GOAL_ORIENTED_PLANNER
- BARGAIN_HUNTER

No duplicates allowed.
No missing personalities allowed.

7. Choices should feel natural.
The correct behavior must NOT be obvious.

8. Keep stories short and simple.

9. Return VALID JSON ONLY.
Do NOT include explanations,
markdown, or extra text.

Categories:
- Spending
- Saving
- Planning
- Bargain Hunting

Output format:

{
  "category": "Saving",
  "story": "You got birthday money. What would you do?",
  "choices": [
    {
      "text": "Buy toys immediately",
      "personality": "IMPULSIVE_SPENDER",
      "weight": 2
    },
    {
      "text": "Save all of it",
      "personality": "PRUDENT_SAVER",
      "weight": 3
    },
    {
      "text": "Spend some and save some",
      "personality": "GOAL_ORIENTED_PLANNER",
      "weight": 2
    },
    {
      "text": "Wait for discounts first",
      "personality": "BARGAIN_HUNTER",
      "weight": 2
    }
  ]
}
"""