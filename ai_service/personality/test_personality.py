from ai_service.personality.profiler import profile_child


sample_features = {
    "spending_pace": "Spends it gradually",
    "tries_to_save": "Yes",
    "reaction_to_100": "Save it all for a future goal",
    "money_mindset": "Balances spending and saving",
    "out_of_money_behavior": "Postpone purchases",
    "spending_categories": [],
    "feeling_after_spending": "Regretful",
    "feeling_when_saving_grows": "Motivated"
}

result = profile_child(sample_features)
print(result)
