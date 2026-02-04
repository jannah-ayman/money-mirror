from ai_service.personality.profiler import profile_child


sample_features = {
    "spending_pace": "Spends it gradually",
    "tries_to_save": "No",
    "reaction_to_100": "Spend part, save part",
    "money_mindset": "Balances spending and saving",
    "out_of_money_behavior": "Postpone purchases",
    "spending_categories": ["Clothes & accessories"],
    "feeling_after_spending": "Neutral",
    "feeling_when_saving_grows": "Doesn't matter much"
}


result = profile_child(sample_features)
print(result)
