using Microsoft.EntityFrameworkCore;
using MoneyMirror.Core.Models;

namespace MoneyMirror.Infrastructure.Data
{
    /// Main database context for Money Mirror application.
    /// Manages all entities and their relationships in SQL Server.
    /// Configured for use with Entity Framework Core 8.0 and SQL Server 2019+.
    public class ApplicationDbContext : DbContext
    {
        /// Constructor that receives database configuration from dependency injection.
        /// Called automatically by ASP.NET Core when DbContext is requested.
        /// <param name="options">Configuration options including connection string</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==================== DbSets (Tables) ====================
        // Each DbSet<T> represents a table in the database
        // Table name matches the property name by convention

        /// Parents table - stores parent/guardian accounts with authentication
        public DbSet<Parent> Parents { get; set; }

        /// Children table - stores child user accounts
        public DbSet<Child> Children { get; set; }

        /// ParentChild junction table - many-to-many relationship between parents and children
        /// Allows one parent to manage multiple children, and one child to have multiple parents
        public DbSet<ParentChild> ParentChildren { get; set; }

        /// Expenses table - logs of purchases made by children
        public DbSet<Expense> Expenses { get; set; }

        /// ExpenseCategory table - predefined categories for classifying expenses
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        /// SavingsGoals table - goals set by children or parents
        public DbSet<SavingsGoal> SavingsGoals { get; set; }

        /// Allowances table - recurring or one-time money given to children
        public DbSet<Allowance> Allowances { get; set; }

        /// Notifications table - alerts sent to parents and children
        public DbSet<Notification> Notifications { get; set; }

        /// PersonalityTypes table - financial personality classifications
        public DbSet<PersonalityType> PersonalityTypes { get; set; }

        /// InitialProfilingQuestionnaire table - parent responses for child setup
        public DbSet<InitialProfilingQuestionnaire> InitialProfilingQuestionnaires { get; set; }

        /// QuizLogs table - child responses to story quizzes
        public DbSet<QuizLog> QuizLogs { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }

        /// StoryQuizTemplate table - predefined story scenarios for quizzes
        public DbSet<StoryQuizTemplate> StoryQuizTemplates { get; set; }

        /// ChildAchievements junction table - tracks which achievements each child has earned
        public DbSet<ChildAchievement> ChildAchievements { get; set; }

        /// AchievementTypes table - predefined achievement/badge definitions
        public DbSet<AchievementType> AchievementTypes { get; set; }

        /// Characters table - space-themed character options
        public DbSet<Character> Characters { get; set; }

        /// CharacterStates table - visual states for each character
        public DbSet<CharacterState> CharacterStates { get; set; }

        /// Moods table - predefined mood emojis for expense logging
        public DbSet<Mood> Moods { get; set; }

        /// Advice table - AI-generated personalized financial tips
        public DbSet<Advice> Advices { get; set; }
        public DbSet<AnalysisAdviceTemplate> AnalysisAdviceTemplates { get; set; }

        /// Transactions table - records all financial movements (allowances, bonuses, expenses)
        public DbSet<Transaction> Transactions { get; set; }

        /// Configure entity relationships, indexes, constraints, and default values.
        /// Called automatically by Entity Framework when creating migrations.
        /// <param name="modelBuilder">Fluent API builder for model configuration</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== PARENT ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Parent>(entity =>
            {
                // Email must be unique across all parents
                entity.HasIndex(p => p.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_Parent_Email_Unique");

                // Default values for SQL Server
                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()"); // SQL Server UTC timestamp

                entity.Property(p => p.IsEmailConfirmed)
                      .HasDefaultValue(false);

                // Precision for timezone-sensitive fields
                entity.Property(p => p.EmailConfirmationCodeExpiry)
                      .HasColumnType("datetime2");

                entity.Property(p => p.PasswordResetCodeExpiry)
                      .HasColumnType("datetime2");

                entity.Property(p => p.RefreshTokenExpiry)
                      .HasColumnType("datetime2");
            });

            // ==================== CHILD ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Child>(entity =>
            {
                // LoginCode must be unique across all children
                entity.HasIndex(c => c.LoginCode)
                      .IsUnique()
                      .HasDatabaseName("IX_Child_LoginCode_Unique");

                // Default values
                entity.Property(c => c.CurrentBalance)
                      .HasDefaultValue(0.00m);

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Configure optional foreign key to PersonalityType
                entity.HasOne(c => c.PersonalityType)
                      .WithMany(pt => pt.Children)
                      .HasForeignKey(c => c.TypeID)
                      .OnDelete(DeleteBehavior.SetNull); // Don't delete child if personality type is removed
                                                         // In the existing Child configuration section, add:
                entity.HasOne(c => c.SelectedCharacter)
                      .WithMany(ch => ch.Children)
                      .HasForeignKey(c => c.CharacterID)
                      .OnDelete(DeleteBehavior.SetNull); // Don't delete child if character removed
            });
            // ==================== CHARACTER ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Character>(entity =>
            {
                // Character name must be unique
                entity.HasIndex(c => c.Name)
                      .IsUnique()
                      .HasDatabaseName("IX_Character_Name_Unique");
            });

            // ==================== CHARACTER STATE ENTITY CONFIGURATION ====================

            modelBuilder.Entity<CharacterState>(entity =>
            {
                // Each character can only have ONE state per screen context
                // (e.g., Nova can't have 2 different dashboard images)
                entity.HasIndex(cs => new { cs.CharacterID, cs.ScreenContext })
                      .IsUnique()
                      .HasDatabaseName("IX_CharacterState_Character_Screen_Unique");

                // Relationship with Character
                entity.HasOne(cs => cs.Character)
                      .WithMany(c => c.CharacterStates)
                      .HasForeignKey(cs => cs.CharacterID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete states if character deleted
            });

            // ==================== PARENT-CHILD RELATIONSHIP (Many-to-Many) ====================

            modelBuilder.Entity<ParentChild>(entity =>
            {
                // Composite primary key (both columns together form the key)
                entity.HasKey(pc => new { pc.ParentID, pc.ChildID });

                // Configure parent side of relationship
                entity.HasOne(pc => pc.Parent)
                      .WithMany(p => p.ParentChildren)
                      .HasForeignKey(pc => pc.ParentID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete links if parent deleted

                // Configure child side of relationship
                entity.HasOne(pc => pc.Child)
                      .WithMany(c => c.ParentChildren)
                      .HasForeignKey(pc => pc.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete links if child deleted
            });

            // ==================== INITIAL PROFILING QUESTIONNAIRE ====================

            modelBuilder.Entity<InitialProfilingQuestionnaire>(entity =>
            {
                // One-to-one relationship with Child
                entity.HasIndex(q => q.ChildID)
                      .IsUnique()
                      .HasDatabaseName("IX_InitialProfiling_ChildID_Unique");

                entity.HasOne(q => q.Child)
                      .WithOne(c => c.InitialProfilingQuestionnaire)
                      .HasForeignKey<InitialProfilingQuestionnaire>(q => q.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete questionnaire if child deleted

                // Relationship with calculated personality type
                entity.HasOne(q => q.CalculatedPersonalityType)
                      .WithMany()
                      .HasForeignKey(q => q.CalculatedTypeID)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allow deleting personality types in use

                // Default values
                entity.Property(q => q.IsCompleted)
                      .HasDefaultValue(false);
            });

            // ==================== EXPENSE ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Expense>(entity =>
            {
                // Index on LogDate for faster date-range queries
                entity.HasIndex(e => e.LogDate)
                      .HasDatabaseName("IX_Expense_LogDate");

                // Composite index for child's expenses ordered by date
                entity.HasIndex(e => new { e.ChildID, e.LogDate })
                      .HasDatabaseName("IX_Expense_Child_LogDate");

                // Relationship with Child
                entity.HasOne(e => e.Child)
                      .WithMany(c => c.Expenses)
                      .HasForeignKey(e => e.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete expenses if child deleted

                // Relationship with ExpenseCategory
                entity.HasOne(e => e.ExpenseCategory)
                      .WithMany(ec => ec.Expenses)
                      .HasForeignKey(e => e.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allow deleting categories in use

                // Relationship with Mood
                entity.HasOne(e => e.Mood)
                      .WithMany(m => m.Expenses)
                      .HasForeignKey(e => e.MoodID)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allow deleting moods in use

                // Default value
                entity.Property(e => e.LogDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ==================== SAVINGS GOAL ENTITY CONFIGURATION ====================

            modelBuilder.Entity<SavingsGoal>(entity =>
            {
                // Index for filtering active goals
                entity.HasIndex(sg => new { sg.ChildID, sg.Status })
                      .HasDatabaseName("IX_SavingsGoal_Child_Status");

                // Relationship with Child (required)
                entity.HasOne(sg => sg.Child)
                      .WithMany(c => c.SavingsGoals)
                      .HasForeignKey(sg => sg.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete goals if child deleted

                // Relationship with Parent (optional - only for challenge goals)
                entity.HasOne(sg => sg.Parent)
                      .WithMany(p => p.ChallengeGoals)
                      .HasForeignKey(sg => sg.ParentID)
                      .OnDelete(DeleteBehavior.SetNull); // Keep child's goal if parent deleted

                // Default values
                entity.Property(sg => sg.CurrentAmount)
                      .HasDefaultValue(0.00m);

                entity.Property(sg => sg.IsChallenge)
                      .HasDefaultValue(false);

                entity.Property(sg => sg.Status)
                      .HasDefaultValue("Active");

                entity.Property(sg => sg.StartDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ==================== ALLOWANCE ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Allowance>(entity =>
            {
                // Index for querying child's allowance history
                entity.HasIndex(a => new { a.ChildID, a.SetDate })
                      .HasDatabaseName("IX_Allowance_Child_SetDate");

                // Relationship with Child
                entity.HasOne(a => a.Child)
                      .WithMany(c => c.Allowances)
                      .HasForeignKey(a => a.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete allowances if child deleted

                // Relationship with Parent
                entity.HasOne(a => a.Parent)
                      .WithMany(p => p.Allowances)
                      .HasForeignKey(a => a.ParentID)
                      .OnDelete(DeleteBehavior.Restrict); // Keep allowance record even if parent deleted

                // Default value
                entity.Property(a => a.SetDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ==================== NOTIFICATION ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Notification>(entity =>
            {
                // Index for querying unread notifications
                entity.HasIndex(n => new { n.ParentID, n.IsRead })
                      .HasDatabaseName("IX_Notification_Parent_IsRead")
                      .HasFilter("[ParentID] IS NOT NULL"); // Partial index (SQL Server 2016+)

                entity.HasIndex(n => new { n.ChildID, n.IsRead })
                      .HasDatabaseName("IX_Notification_Child_IsRead")
                      .HasFilter("[ChildID] IS NOT NULL");

                // Relationship with Parent (optional)
                entity.HasOne(n => n.Parent)
                      .WithMany(p => p.Notifications)
                      .HasForeignKey(n => n.ParentID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete notifications if parent deleted

                // Relationship with Child (optional)
                entity.HasOne(n => n.Child)
                      .WithMany(c => c.Notifications)
                      .HasForeignKey(n => n.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete notifications if child deleted

                // Default values
                entity.Property(n => n.IsRead)
                      .HasDefaultValue(false);

                entity.Property(n => n.SentDate)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(n => n.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ==================== QUIZ LOG ENTITY CONFIGURATION ====================
            modelBuilder.Entity<QuizLog>(entity =>
            {
                entity.HasIndex(ql => new { ql.ChildID, ql.CompletedDate })
                      .HasDatabaseName("IX_QuizLog_Child_CompletedDate");

                // Unique: one answer per story per child
                entity.HasIndex(ql => new { ql.ChildID, ql.StoryID })
                      .IsUnique()
                      .HasDatabaseName("IX_QuizLog_Child_Story_Unique");

                entity.HasOne(ql => ql.Child)
                      .WithMany(c => c.QuizLogs)
                      .HasForeignKey(ql => ql.ChildID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ql => ql.StoryQuizTemplate)
                      .WithMany(s => s.QuizLogs)
                      .HasForeignKey(ql => ql.StoryID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ql => ql.QuizAnswer)
                      .WithMany(a => a.QuizLogs)
                      .HasForeignKey(ql => ql.AnswerID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ql => ql.CompletedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
            modelBuilder.Entity<QuizAnswer>(entity =>
            {
                entity.HasOne(a => a.StoryQuizTemplate)
                      .WithMany(s => s.Answers)
                      .HasForeignKey(a => a.StoryID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.PersonalityType)
                      .WithMany()
                      .HasForeignKey(a => a.PersonalityTypeID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==================== CHILD ACHIEVEMENT (Many-to-Many) ====================

            modelBuilder.Entity<ChildAchievement>(entity =>
            {
                // Composite primary key
                entity.HasKey(ca => new { ca.ChildID, ca.AchievementTypeID });

                // Relationship with Child
                entity.HasOne(ca => ca.Child)
                      .WithMany(c => c.ChildAchievements)
                      .HasForeignKey(ca => ca.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete achievements if child deleted

                // Relationship with AchievementType
                entity.HasOne(ca => ca.AchievementType)
                      .WithMany(at => at.ChildAchievements)
                      .HasForeignKey(ca => ca.AchievementTypeID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete link if achievement type deleted

                // Default value
                entity.Property(ca => ca.EarnedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });



            // ==================== ADVICE ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Advice>(entity =>
            {
                // Index for querying unread advice
                entity.HasIndex(a => new { a.ChildID, a.IsRead })
                      .HasDatabaseName("IX_Advice_Child_IsRead");

                // Relationship with Child
                entity.HasOne(a => a.Child)
                      .WithMany(c => c.Advices)
                      .HasForeignKey(a => a.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete advice if child deleted

                // Default values
                entity.Property(a => a.IsRead)
                      .HasDefaultValue(false);

                entity.Property(a => a.GeneratedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
            modelBuilder.Entity<AnalysisAdviceTemplate>().HasData(
                // ==================== ALERTS ====================
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 1,
                    Title = "Emotional Spending Alert",
                    Description = "Your child spends a large portion of their money when in a specific emotional state, which may indicate mood-driven impulse purchases.",
                    ActionSteps = "[\"Introduce a 24-hour waiting rule before any purchase over 20 EGP\",\"Talk to your child about how their mood affects their decisions\",\"Create a wish list to revisit purchases when they feel calmer\"]",
                    Type = "Alert",
                    TriggerKey = "HIGH_MOOD_SPENDING",
                    Priority = 1
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 2,
                    Title = "Low Savings Rate",
                    Description = "Your child is saving very little of their allowance, spending most of it before the next payment cycle.",
                    ActionSteps = "[\"Set an automatic savings rule: put aside 20% of allowance immediately\",\"Use a visible savings goal to motivate consistent saving\",\"Discuss what they are saving toward to build purpose\"]",
                    Type = "Alert",
                    TriggerKey = "LOW_SAVINGS_RATIO",
                    Priority = 1
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 3,
                    Title = "High Spending Frequency",
                    Description = "Your child is making purchases very frequently relative to their allowance cycle, which may lead to running out of money early.",
                    ActionSteps = "[\"Help your child plan their spending at the start of each allowance cycle\",\"Set a weekly spending limit together\",\"Review purchases together at the end of each week\"]",
                    Type = "Alert",
                    TriggerKey = "HIGH_SPENDING_FREQUENCY",
                    Priority = 2
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 4,
                    Title = "Category Over-Focus",
                    Description = "Your child is spending the majority of their money on a single category, showing limited variety in their purchasing decisions.",
                    ActionSteps = "[\"Encourage your child to explore other categories like books or school supplies\",\"Set a soft limit on spending in their dominant category per cycle\",\"Talk about balancing needs vs. wants\"]",
                    Type = "Alert",
                    TriggerKey = "IMPULSIVE_CATEGORY_FOCUS",
                    Priority = 2
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 5,
                    Title = "No Active Savings Goals",
                    Description = "Your child currently has no active savings goals, which removes a key motivation to save and plan ahead.",
                    ActionSteps = "[\"Sit together and pick one thing your child wants to save for\",\"Set a realistic target amount and timeline\",\"Check in on progress weekly to keep motivation high\"]",
                    Type = "Alert",
                    TriggerKey = "NO_ACTIVE_GOALS",
                    Priority = 2
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 6,
                    Title = "Balance Draining Fast",
                    Description = "Your child's balance has dropped significantly since their last allowance, suggesting rapid spending after receiving money.",
                    ActionSteps = "[\"Discuss the concept of making money last through the full cycle\",\"Try splitting the allowance into spending and saving portions immediately\",\"Review what large purchases were made and whether they were planned\"]",
                    Type = "Alert",
                    TriggerKey = "LOW_BALANCE_DRAIN",
                    Priority = 1
                },
                // ==================== STRENGTHS ====================
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 7,
                    Title = "Goal Achiever",
                    Description = "Your child has been completing savings goals consistently, showing strong financial discipline and follow-through.",
                    ActionSteps = "[\"Celebrate this achievement with non-monetary praise\",\"Help set a new, slightly more ambitious goal\",\"Share their success to reinforce the behavior\"]",
                    Type = "Strength",
                    TriggerKey = "GOAL_STREAK",
                    Priority = 1
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 8,
                    Title = "Consistent Tracker",
                    Description = "Your child has been logging their expenses regularly, building a strong habit of financial awareness.",
                    ActionSteps = "[\"Praise their consistency to reinforce the habit\",\"Review logged expenses together to deepen their understanding\",\"Use the data to help them spot their own patterns\"]",
                    Type = "Strength",
                    TriggerKey = "CONSISTENT_LOGGING",
                    Priority = 2
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 9,
                    Title = "Emotionally Balanced Spender",
                    Description = "Your child's spending is distributed across different moods without any single emotional state dominating their purchases.",
                    ActionSteps = "[\"Acknowledge this balance as a sign of developing impulse control\",\"Continue open conversations about how feelings relate to money decisions\",\"Encourage them to keep reflecting on their mood when spending\"]",
                    Type = "Strength",
                    TriggerKey = "BALANCED_MOOD_SPENDING",
                    Priority = 3
                },
                new AnalysisAdviceTemplate
                {
                    AdviceTemplateID = 10,
                    Title = "Steady Saver",
                    Description = "Your child has made meaningful progress toward at least one savings goal, showing patience and planning ability.",
                    ActionSteps = "[\"Highlight how close they are to their goal to keep momentum\",\"Discuss what they will do once they reach it\",\"Suggest adding a stretch goal for extra motivation\"]",
                    Type = "Strength",
                    TriggerKey = "SAVING_PROGRESS",
                    Priority = 2
                }
            );
            // ==================== STORY QUIZ TEMPLATE CONFIGURATION ====================

            modelBuilder.Entity<StoryQuizTemplate>(entity =>
            {
                entity.HasIndex(s => new { s.TargetAgeMin, s.TargetAgeMax })
                      .HasDatabaseName("IX_StoryQuiz_AgeRange");

                entity.Property(s => s.CreatedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ==================== TRANSACTION ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => new { t.ChildID, t.TransactionDate })
                      .HasDatabaseName("IX_Transaction_Child_TransactionDate");

                entity.HasIndex(t => new { t.Type, t.TransactionDate })
                      .HasDatabaseName("IX_Transaction_Type_TransactionDate");

                // ✅ FIX HERE
                entity.HasOne(t => t.Child)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(t => t.ChildID)
                      .OnDelete(DeleteBehavior.NoAction); // or Restrict

                entity.HasOne(t => t.Parent)
                      .WithMany(p => p.InitiatedTransactions)
                      .HasForeignKey(t => t.ParentID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(t => t.Allowance)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AllowanceID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(t => t.TransactionDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
            // ==================== SEED DATA (EXPENSE CATEGORIES) ====================
            modelBuilder.Entity<ExpenseCategory>().HasData(
                new ExpenseCategory { CategoryID = 1, Name = "Snacks / Food" },
                new ExpenseCategory { CategoryID = 2, Name = "Games / Toys" },
                new ExpenseCategory { CategoryID = 3, Name = "Gifts" },
                new ExpenseCategory { CategoryID = 4, Name = "School Supplies" },
                new ExpenseCategory { CategoryID = 5, Name = "Other" }
            );

            // ==================== SEED DATA (CHARACTERS) ====================
            modelBuilder.Entity<Character>().HasData(
                new Character { CharacterID = 1, Name = "Nova", Description = "Cool astronaut who loves street style and music", DefaultImageUrl = "/images/characters/nova/profile.png" },
                new Character { CharacterID = 2, Name = "Cosmo", Description = "Ninja superhero astronaut always ready for action", DefaultImageUrl = "/images/characters/cosmo/profile.png" },
                new Character { CharacterID = 3, Name = "Luna", Description = "Graceful ballerina astronaut in a pink skirt", DefaultImageUrl = "/images/characters/luna/profile.png" },
                new Character { CharacterID = 4, Name = "Stella", Description = "Laid back astronaut in a hoodie who loves bubblegum", DefaultImageUrl = "/images/characters/stella/profile.png" }
            );

            // ==================== SEED DATA (CHARACTER STATES) ====================
            modelBuilder.Entity<CharacterState>().HasData(
                new CharacterState { StateID = 6, CharacterID = 1, ScreenContext = "Expenses", ImageUrl = "/images/characters/nova/expenses.png", Message = "Let's see what you copped this week, astronaut." },
                new CharacterState { StateID = 7, CharacterID = 2, ScreenContext = "Expenses", ImageUrl = "/images/characters/cosmo/expenses.png", Message = "Time to investigate your spending like a true hero." },
                new CharacterState { StateID = 8, CharacterID = 3, ScreenContext = "Expenses", ImageUrl = "/images/characters/luna/expenses.png", Message = "Every purchase tells a story. Let's review yours." },
                new CharacterState { StateID = 9, CharacterID = 4, ScreenContext = "Expenses", ImageUrl = "/images/characters/stella/expenses.png", Message = "No stress, let's just vibe and see what you bought." },
                new CharacterState { StateID = 10, CharacterID = 1, ScreenContext = "Goals",  ImageUrl = "/images/characters/nova/goals.png", Message = "Your bank account looking real nice right now." },
                new CharacterState { StateID = 11, CharacterID = 2, ScreenContext = "Goals", ImageUrl = "/images/characters/cosmo/goals.png", Message = "Your savings power is charging up fast." },
                new CharacterState { StateID = 12, CharacterID = 3, ScreenContext = "Goals", ImageUrl = "/images/characters/luna/goals.png", Message = "Your savings are dancing beautifully toward your dreams." },
                new CharacterState { StateID = 14, CharacterID = 4, ScreenContext = "Goals", ImageUrl = "/images/characters/stella/goals.png", Message = "Pretty cool how your money's stacking up like that." },
                new CharacterState { StateID = 15, CharacterID = 1, ScreenContext = "Profile", ImageUrl = "/images/characters/nova/profile.png", Message = "Yo, what's good? Time to check those space credits." },
                new CharacterState { StateID = 16, CharacterID = 2, ScreenContext = "Profile", ImageUrl = "/images/characters/cosmo/profile.png", Message = "Looking strong, money warrior. Keep training." },
                new CharacterState { StateID = 17, CharacterID = 3, ScreenContext = "Profile", ImageUrl = "/images/characters/luna/profile.png", Message = "Welcome back, little star. Shall we begin?" },
                new CharacterState { StateID = 18, CharacterID = 4, ScreenContext = "Profile", ImageUrl = "/images/characters/stella/profile.png", Message = "Hey there, space buddy. Just chilling and checking in." },
                new CharacterState { StateID = 19, CharacterID = 1, ScreenContext = "Badges", ImageUrl = "/images/characters/nova/badges.png", Message = "Another badge? You're on fire with this." },
                new CharacterState { StateID = 20, CharacterID = 2, ScreenContext = "Badges", ImageUrl = "/images/characters/cosmo/badges.png", Message = "Another victory unlocked. You're unstoppable." },
                new CharacterState { StateID = 23, CharacterID = 3, ScreenContext = "Badges", ImageUrl = "/images/characters/luna/badges.png", Message = "Each achievement is like a perfect spin." },
                new CharacterState { StateID = 29, CharacterID = 4, ScreenContext = "Badges", ImageUrl = "/images/characters/stella/badges.png", Message = "Nice, another one. You're doing your thing." },
                new CharacterState { StateID = 32, CharacterID = 1, ScreenContext = "Quiz", ImageUrl = "/images/characters/nova/quiz.png", Message = "Aight, let's test that money brain of yours." },
                new CharacterState { StateID = 33, CharacterID = 2, ScreenContext = "Quiz", ImageUrl = "/images/characters/cosmo/quiz.png", Message = "Think fast, space cadet. Show me your skills." },
                new CharacterState { StateID = 34, CharacterID = 3, ScreenContext = "Quiz", ImageUrl = "/images/characters/luna/quiz.png", Message = "Let's gracefully glide through these questions together." },
                new CharacterState { StateID = 35, CharacterID = 4, ScreenContext = "Quiz", ImageUrl = "/images/characters/stella/quiz.png", Message = "Take it easy, no rush. You got this." }
            );

            // ==================== SEED DATA (MOODS) ====================
            modelBuilder.Entity<Mood>().HasData(
                new Mood { MoodID = 1, Description = "Happy" },
                new Mood { MoodID = 2, Description = "Sad" },
                new Mood { MoodID = 3, Description = "Neutral" },
                new Mood { MoodID = 4, Description = "Excited" },
                new Mood { MoodID = 5, Description = "Regretful" }
            );
            modelBuilder.Entity<AchievementType>().HasData(
                new AchievementType { AchievementTypeID = 1, Name = "First Step", Description = "Answered your first quiz question!", IconURL = "/images/badges/first-step.png", Category = "Quiz", Threshold = 1 },
                new AchievementType { AchievementTypeID = 2, Name = "Quiz Explorer", Description = "Answered 10 quiz questions.", IconURL = "/images/badges/quiz-explorer.png", Category = "Quiz", Threshold = 10 },
                new AchievementType { AchievementTypeID = 3, Name = "Quiz Master", Description = "Answered 20 quiz questions.", IconURL = "/images/badges/quiz-master.png", Category = "Quiz", Threshold = 20 },
                new AchievementType { AchievementTypeID = 4, Name = "Quiz Legend", Description = "Answered 50 quiz questions!", IconURL = "/images/badges/quiz-legend.png", Category = "Quiz", Threshold = 50 },
                new AchievementType { AchievementTypeID = 5, Name = "Goal Getter", Description = "Completed your first savings goal!", IconURL = "/images/badges/goal-getter.png", Category = "Goal", Threshold = 1 },
                new AchievementType { AchievementTypeID = 6, Name = "Determined", Description = "Completed 3 savings goals.", IconURL = "/images/badges/determined.png", Category = "Goal", Threshold = 3 },
                new AchievementType { AchievementTypeID = 7, Name = "Achiever", Description = "Completed 5 savings goals.", IconURL = "/images/badges/achiever.png", Category = "Goal", Threshold = 5 },
                new AchievementType { AchievementTypeID = 8, Name = "Champion", Description = "Completed 10 savings goals!", IconURL = "/images/badges/champion.png", Category = "Goal", Threshold = 10 },
                new AchievementType { AchievementTypeID = 9, Name = "First Purchase", Description = "Logged your first expense.", IconURL = "/images/badges/first-purchase.png", Category = "Expense", Threshold = 1 },
                new AchievementType { AchievementTypeID = 10, Name = "Expense Tracker", Description = "Logged 20 expenses.", IconURL = "/images/badges/expense-tracker.png", Category = "Expense", Threshold = 20 },
                new AchievementType { AchievementTypeID = 11, Name = "Money Logger", Description = "Logged 40 expenses.", IconURL = "/images/badges/money-logger.png", Category = "Expense", Threshold = 40 },
                new AchievementType { AchievementTypeID = 12, Name = "Financial Pro", Description = "Logged 100 expenses!", IconURL = "/images/badges/financial-pro.png", Category = "Expense", Threshold = 100 }
            );

            // ==================== SEED DATA (PERSONALITY TYPES) ====================
            modelBuilder.Entity<PersonalityType>().HasData(
                new PersonalityType
                {
                    TypeID = 1,
                    ParentName = "Pending Analysis",
                    ChildName = "Money Explorer",
                    Desc = "We're still getting to know your child's financial personality. As they use the app and log expenses, we'll build a complete picture of their money habits and provide personalized guidance.",
                    Traits = "[\"Discovering spending patterns\", \"Building financial profile\", \"Learning money habits\"]",
                    FunFacts = "Every money expert started somewhere — you're just getting started!",
                    StaticRecommendation = "[\"Keep encouraging your child to log their expenses regularly\", \"Guide your child to try saving for a specific goal\", \"Have your child complete the story quizzes to help us better understand their money personality\"]"
                },
                new PersonalityType
                {
                    TypeID = 2,
                    ParentName = "Impulsive Spender",
                    ChildName = "Speedy Spender",
                    Desc = "Quick purchases driven by excitement, low savings ratios.",
                    Traits = "[\"Buys quickly\",\"Gets excited about new things\",\"Struggles to save\"]",
                    FunFacts = "Did you know? Speedy Spenders are super fun and spontaneous — the trick is to pause for just one day before buying!",
                    StaticRecommendation = "[\"Encourage a 24-hour waiting rule before they make non-essential purchases.\", \"Introduce a visual savings jar or progress bar so they can see their money build up.\", \"Work together to create a simple shopping list before visiting stores or online apps.\", \"Suggest setting aside a flat 20% of their allowance instantly into savings before spending any.\"]"
                },
                new PersonalityType
                {
                    TypeID = 3,
                    ParentName = "Prudent Saver",
                    ChildName = "Treasure Keeper",
                    Desc = "High savings ratios and deliberate spending decisions.",
                    Traits = "[\"Thinks before buying\",\"Saves consistently\",\"Rarely regrets purchases\"]",
                    FunFacts = "Did you know? Treasure Keepers are rare — only the wisest kids know how to grow their coins into something amazing!",
                    StaticRecommendation = "[\"Help them set exciting, long-term savings goals so they don't hold onto money out of fear.\", \"Give them permission to enjoy some 'fun spending' to avoid eventual saving burnout.\", \"Introduce basic age-appropriate concepts of investing or earning interest on accumulated funds.\"]"
                },
                new PersonalityType
                {
                    TypeID = 4,
                    ParentName = "Goal-Oriented Planner",
                    ChildName = "Dream Builder",
                    Desc = "Balanced approach to spending and saving, with steady goal contributions. Plans purchases carefully.",
                    Traits = "[\"Creates clear savings goals\", \"Balances fun spending with saving\", \"Tracks progress regularly\", \"Plans purchases in advance\", \"Stays motivated by dreams\"]",
                    FunFacts = "Did you know? Dream Builders are natural achievers — every coin you save is one step closer to your dream!",
                    StaticRecommendation = "[\"Help them break down very large, daunting savings goals into smaller, reachable milestones.\", \"Celebrate or match their savings when they cross a major milestone to reward consistency.\", \"Keep their targeted goals visually prominent in conversation to sustain their natural planning habits.\"]",
                },
                new PersonalityType
                {
                    TypeID = 5,
                    ParentName = "Bargain Hunter",
                    ChildName = "Deal Detective",
                    Desc = "Emphasizes value and deals, strategic spending. Loves finding the best prices and getting good value.",
                    Traits = "[\"Compares prices before buying\", \"Loves finding good deals\", \"Waits for sales\", \"Values getting the most for money\", \"Shares deals with others\"]",
                    FunFacts = "Did you know? Deal Detectives have a superpower — they can spot a great deal from a mile away!",
                    StaticRecommendation = "[\"Remind them to make a strict shopping list so they don't buy things simply because they are 'on sale'.\", \"Challenge them to find coupon codes or comparison shop to engage their detective strengths productively.\", \"Teach them about quality vs. price so they understand that cheap doesn't always mean high value.\"]"
                }
            );
        }
    }
}