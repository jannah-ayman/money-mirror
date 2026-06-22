import random

BLOCKED_WORDS = [
    "kill",
    "suicide",
    "self harm",
    "drug",
    "drugs",
    "alcohol",
    "gambling",
    "casino",
    "weapon",
    "gun",
    "bomb",
    "sex",
    "porn",
    "politics",
    "war"
]

MONEY_MIRROR_KEYWORDS = [
    "money",
    "save",
    "saving",
    "budget",
    "spend",
    "spending",
    "allowance",
    "goal",
    "goals",
    "earn",
    "earned",
    "buy",
    "shopping",
    "needs",
    "wants",
    "expense",
    "expenses",
    "balance",
    "coins",
    "wallet"
]


def is_safe_child_message(message):
    message = message.lower()
    return not any(word in message for word in BLOCKED_WORDS)


def is_money_mirror_related(message):
    message = message.lower()
    return any(keyword in message for keyword in MONEY_MIRROR_KEYWORDS)


def blocked_child_response():

    responses = [

        "Oops! 😊 Let's use kind words and focus on becoming money superheroes! 🦸💰",

        "I'm here to help with saving money and reaching your dreams! 🌟",

        "Let's talk about fun money adventures instead! 🚀💰",

        "How about we learn something cool about saving and smart spending? 😊",

        "Money Mirror loves helping kids make smart choices! 💵✨",

        "Let's turn our attention to saving, goals, and becoming money champions! 🏆"
    ]

    return random.choice(responses)


def unrelated_question_response():

    responses = [

        "That's an interesting question! 😄 I'm best at helping with money and smart spending. 💰",

        "Hmm... I know lots about saving money and reaching goals! 🌟",

        "Let's go on a money adventure! 🚀 Ask me about saving or shopping wisely.",

        "I love talking about allowance, goals, and smart choices! 😊",

        "What would you like to save for? A toy? A bike? Something special? 🚲✨",

        "Money Mirror is your friendly money coach! 💵 Ask me anything about saving and spending wisely.",

        "Did you know small savings can lead to big dreams? 🌈💰",

        "I'm an expert on money habits! Let's talk about saving and goals. 😄"
    ]

    return random.choice(responses)