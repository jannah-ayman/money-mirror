import os

from dotenv import load_dotenv
from openai import OpenAI


load_dotenv()


client = OpenAI(
    api_key=os.getenv("CLAUDE_API_KEY"),
    base_url="https://agentrouter.org/v1",
)


MODEL = os.getenv(
    "CLAUDE_MODEL",
    "claude-3-5-sonnet"
)


def generate_questions_from_llm(
    prompt: str
) -> str:
    """
    Generate quiz questions
    from LLM safely.
    """

    response = client.chat.completions.create(
        model=MODEL,
        messages=[
            {
                "role": "system",
                "content": (
                    "You are a JSON generator. "
                    "Return ONLY valid JSON."
                )
            },
            {
                "role": "user",
                "content": prompt
            }
        ],
        temperature=0.8,
        max_tokens=3000,
        stream=False
    )

    return response.choices[
        0
    ].message.content