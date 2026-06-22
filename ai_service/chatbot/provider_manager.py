# from .gemini_provider import gemini_response
# from .groq_provider import groq_response
# from .openrouter_provider import openrouter_response


# def get_response(prompt):

#     # First provider: Gemini
#     try:

#         print("Using Gemini")

#         return gemini_response(prompt)

#     except Exception as e:

#         print("Gemini failed:", str(e))

#     # Second provider: Groq
#     try:

#         print("Using Groq")

#         return groq_response(prompt)

#     except Exception as e:

#         print("Groq failed:", str(e))

#     # Third provider: OpenRouter
#     try:

#         print("Using OpenRouter")

#         return openrouter_response(prompt)

#     except Exception as e:

#         print("OpenRouter failed:", str(e))

#     return (
#         "I'm sorry, I'm having trouble answering right now. "
#         "Please try again later."
#     )



from .gemini_provider import gemini_response
from .groq_provider import groq_response
from .openrouter_provider import openrouter_response


def get_response(prompt):

    # -------------------
    # Gemini (Primary)
    # -------------------
    try:
        print("Using Gemini")
        return gemini_response(prompt)

    except Exception as e:
        print("Gemini failed:", str(e))

    # -------------------
    # Groq (Fallback 1)
    # -------------------
    try:
        print("Using Groq")
        return groq_response(prompt)

    except Exception as e:
        print("Groq failed:", str(e))

    # -------------------
    # OpenRouter (Fallback 2)
    # -------------------
    try:
        print("Using OpenRouter")
        return openrouter_response(prompt)

    except Exception as e:
        print("OpenRouter failed:", str(e))

    return "Sorry, I'm having trouble responding right now."