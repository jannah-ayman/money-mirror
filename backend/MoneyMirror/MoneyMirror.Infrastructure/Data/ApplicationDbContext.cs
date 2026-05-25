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

            modelBuilder.Entity<AchievementType>().HasData(
    // Quiz badges
    new AchievementType { AchievementTypeID = 1, Name = "First Step", IconURL = "/images/badges/first-step.png", Category = "Quiz", Threshold = 1 },
    new AchievementType { AchievementTypeID = 2, Name = "Quiz Explorer", IconURL = "/images/badges/quiz-explorer.png", Category = "Quiz", Threshold = 10 },
    new AchievementType { AchievementTypeID = 3, Name = "Quiz Master",  IconURL = "/images/badges/quiz-master.png", Category = "Quiz", Threshold = 20 },
    new AchievementType { AchievementTypeID = 4, Name = "Quiz Legend", IconURL = "/images/badges/quiz-legend.png", Category = "Quiz", Threshold = 50 },

    // Goal badges
    new AchievementType { AchievementTypeID = 5, Name = "Goal Getter",  IconURL = "/images/badges/goal-getter.png", Category = "Goal", Threshold = 1 },
    new AchievementType { AchievementTypeID = 6, Name = "Determined",  IconURL = "/images/badges/determined.png", Category = "Goal", Threshold = 3 },
    new AchievementType { AchievementTypeID = 7, Name = "Achiever", IconURL = "/images/badges/achiever.png", Category = "Goal", Threshold = 5 },
    new AchievementType { AchievementTypeID = 8, Name = "Champion",  IconURL = "/images/badges/champion.png", Category = "Goal", Threshold = 10 },

    // Expense badges
    new AchievementType { AchievementTypeID = 9, Name = "First Purchase",IconURL = "/images/badges/first-purchase.png", Category = "Expense", Threshold = 1 },
    new AchievementType { AchievementTypeID = 10, Name = "Expense Tracker",IconURL = "/images/badges/expense-tracker.png", Category = "Expense", Threshold = 20 },
    new AchievementType { AchievementTypeID = 11, Name = "Money Logger", IconURL = "/images/badges/money-logger.png", Category = "Expense", Threshold = 40 },
    new AchievementType { AchievementTypeID = 12, Name = "Financial Pro",  IconURL = "/images/badges/financial-pro.png", Category = "Expense", Threshold = 100 }
);
            // ==================== SEED DATA (Optional) ====================
            // Uncomment if you want to seed initial data like categories, moods, etc.

            /*
            // Seed Expense Categories
            modelBuilder.Entity<ExpenseCategory>().HasData(
                new ExpenseCategory { CategoryID = 1, Name = "Toys" },
                new ExpenseCategory { CategoryID = 2, Name = "Food & Snacks" },
                new ExpenseCategory { CategoryID = 3, Name = "Books" },
                new ExpenseCategory { CategoryID = 4, Name = "Clothes" },
                new ExpenseCategory { CategoryID = 5, Name = "Entertainment" }
            );

            // Seed Moods
            modelBuilder.Entity<Mood>().HasData(
                new Mood { MoodID = 1, Emoji = "😊", Description = "Happy" },
                new Mood { MoodID = 2, Emoji = "😢", Description = "Sad" },
                new Mood { MoodID = 3, Emoji = "😐", Description = "Neutral" },
                new Mood { MoodID = 4, Emoji = "😍", Description = "Excited" },
                new Mood { MoodID = 5, Emoji = "😔", Description = "Regretful" }
            );
            */
        }
    }
}