import json
import os
import time

# Import the functions you already created in your project files
from llm_client import generate_questions_from_llm
from quiz_generator import parse_generated_questions
from prompt_templates import QUIZ_GENERATION_PROMPT

def start_generation_process():
    # 1. Split diverse topics into 10 batches to ensure maximum variety
    topics_per_batch = [
        "school activities, lunch decisions, and school events",
        "toys, video games, online games, and digital purchases",
        "subscriptions, rewards programs, and advertisements",
        "shopping with friends, family outings, and class trips",
        "sports, hobbies, collections, and team activities",
        "fundraising, saving for something special, and planning ahead",
        "helping others, sharing resources, and unexpected opportunities",
        "emotional purchases, peer influence, and entertainment decisions",
        "waiting for better opportunities and comparing alternatives",
        "limited-time offers, choosing gifts, and choosing between options"
    ]

    # Main list that will store all 500 generated questions
    all_questions = []

    # Auto-incrementing question ID counter
    question_id_counter = 1

    print("🚀 Starting the 500-question generation process...")

    # 2. Run the generation loop 10 times
    for i in range(10):
        batch_number = i + 1
        current_topic = topics_per_batch[i]

        print(
            f"\n🔄 Processing batch ({batch_number}/10) | "
            f"Focus topic: [{current_topic}]"
        )

        # 3. Combine the batch-specific instructions with the base prompt
        dynamic_prompt = f"""
        {QUIZ_GENERATION_PROMPT}

        CRITICAL DIRECTION FOR THIS BATCH:
        This is Batch #{batch_number} out of 10.
        To avoid duplication across the entire system, you MUST base your 50 scenarios primarily on these themes: {current_topic}.
        Generate EXACTLY 50 unique questions.
        """

        # Retry mechanism in case parsing or validation fails
        success = False
        attempts = 3

        while attempts > 0 and not success:
            try:
                # Send the customized prompt to the LLM
                raw_response = generate_questions_from_llm(dynamic_prompt)

                # Parse, clean, and validate the generated questions
                parsed_questions = parse_generated_questions(raw_response)

                if parsed_questions and len(parsed_questions) > 0:
                    # Assign sequential IDs to each question
                    for question in parsed_questions:
                        question["question_id"] = question_id_counter
                        all_questions.append(question)
                        question_id_counter += 1

                    print(
                        f"✅ Batch {batch_number} completed successfully! "
                        f"Added {len(parsed_questions)} questions."
                    )
                    success = True

                else:
                    print(
                        f"⚠️ Batch {batch_number} failed validation. "
                        f"Retrying..."
                    )
                    attempts -= 1
                    time.sleep(2)

            except Exception as e:
                print(f"❌ API error occurred: {e}")
                attempts -= 1
                time.sleep(2)

        if not success:
            print(
                f"🚨 Failed to generate Batch {batch_number} "
                f"after 3 attempts."
            )

        # Short delay between requests to avoid rate limits
        time.sleep(1)

    # 4. Save all generated questions into a single JSON file
    output_file = "final_financial_quiz_500.json"

    with open(output_file, "w", encoding="utf-8") as f:
        json.dump(all_questions, f, indent=4, ensure_ascii=False)

    print("\n🎉 Generation process completed!")
    print(f"📁 Output saved to: {output_file}")
    print(f"🔢 Total questions generated: {len(all_questions)}")

if __name__ == "__main__":
    start_generation_process()