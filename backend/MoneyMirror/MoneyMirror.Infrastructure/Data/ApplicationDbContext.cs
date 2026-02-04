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

        /// StoryQuizTemplate table - predefined story scenarios for quizzes
        public DbSet<StoryQuizTemplate> StoryQuizTemplates { get; set; }

        /// ChildAchievements junction table - tracks which achievements each child has earned
        public DbSet<ChildAchievement> ChildAchievements { get; set; }

        /// AchievementTypes table - predefined achievement/badge definitions
        public DbSet<AchievementType> AchievementTypes { get; set; }

        /// CharacterStats table - predefined character animation states
        public DbSet<CharacterStats> CharacterStats { get; set; }

        /// ChildCharacterStats table - logs of character reactions shown to children
        public DbSet<ChildCharacterStats> ChildCharacterStats { get; set; }

        /// Moods table - predefined mood emojis for expense logging
        public DbSet<Mood> Moods { get; set; }

        /// Advice table - AI-generated personalized financial tips
        public DbSet<Advice> Advices { get; set; }

        /// Transactions table - records all financial movements (allowances, bonuses, expenses)
        public DbSet<Transaction> Transactions { get; set; }

        /// Characters table - available character types for children to select
        public DbSet<Character> Characters { get; set; }

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
                entity.Property(p => p.EmailConfirmationTokenExpiry)
                      .HasColumnType("datetime2");

                entity.Property(p => p.PasswordResetTokenExpiry)
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

                // Configure optional foreign key to Character
                entity.HasOne(c => c.Character)
                      .WithMany(ch => ch.Children)
                      .HasForeignKey(c => c.CharacterID)
                      .OnDelete(DeleteBehavior.SetNull); // Don't delete child if character is removed
            });

            // ==================== CHARACTER ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Character>(entity =>
            {
                // CharacterType must be unique
                entity.HasIndex(ch => ch.CharacterType)
                      .IsUnique()
                      .HasDatabaseName("IX_Character_CharacterType_Unique");

                // Default value
                entity.Property(ch => ch.IsActive)
                      .HasDefaultValue(true);
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
                // Index for analytics queries
                entity.HasIndex(ql => new { ql.ChildID, ql.CompletedDate })
                      .HasDatabaseName("IX_QuizLog_Child_CompletedDate");

                // Relationship with Child
                entity.HasOne(ql => ql.Child)
                      .WithMany(c => c.QuizLogs)
                      .HasForeignKey(ql => ql.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete quiz logs if child deleted

                // Relationship with StoryQuizTemplate
                entity.HasOne(ql => ql.StoryQuizTemplate)
                      .WithMany()
                      .HasForeignKey(ql => ql.StoryID)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allow deleting templates in use

                // Relationship with PersonalityType
                entity.HasOne(ql => ql.PersonalityType)
                      .WithMany(pt => pt.QuizLogs)
                      .HasForeignKey(ql => ql.TypeID)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allow deleting types in use

                // Default value
                entity.Property(ql => ql.CompletedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
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

            // ==================== CHILD CHARACTER STATS CONFIGURATION ====================

            modelBuilder.Entity<ChildCharacterStats>(entity =>
            {
                // Index for analytics
                entity.HasIndex(ccs => new { ccs.ChildID, ccs.TriggerEvent })
                      .HasDatabaseName("IX_ChildCharacterStats_Child_Trigger");

                // Relationship with Child
                entity.HasOne(ccs => ccs.Child)
                      .WithMany(c => c.ChildCharacterStats)
                      .HasForeignKey(ccs => ccs.ChildID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete stats if child deleted

                // Relationship with CharacterStats
                entity.HasOne(ccs => ccs.CharacterStats)
                      .WithMany(cs => cs.ChildCharacterStats)
                      .HasForeignKey(ccs => ccs.StatsID)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allow deleting character states in use
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
                // Index for filtering by age range
                entity.HasIndex(sqt => new { sqt.TargetAgeMin, sqt.TargetAgeMax })
                      .HasDatabaseName("IX_StoryQuiz_AgeRange");

                // Default value
                entity.Property(sqt => sqt.CreatedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ==================== TRANSACTION ENTITY CONFIGURATION ====================

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => new { t.ChildID, t.TransactionDate })
                      .HasDatabaseName("IX_Transaction_Child_TransactionDate");

                entity.HasIndex(t => new { t.Type, t.TransactionDate })
                      .HasDatabaseName("IX_Transaction_Type_TransactionDate");

                entity.HasOne(t => t.Child)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(t => t.ChildID)
                      .OnDelete(DeleteBehavior.NoAction);

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

            // ==================== CHARACTER SEED DATA ====================
            modelBuilder.Entity<Character>().HasData(
                new Character
                {
                    CharacterID = 1,
                    CharacterType = "Nova",
                    DisplayName = "Nova the Explorer",
                    Description = "Energetic and loves adventures! Nova is always excited to help you reach your goals! 🚀",
                    BasePath = "/characters/nova",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new Character
                {
                    CharacterID = 2,
                    CharacterType = "Luna",
                    DisplayName = "Luna the Thinker",
                    Description = "Calm and thoughtful! Luna helps you make smart decisions about your money. 🌙",
                    BasePath = "/characters/luna",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new Character
                {
                    CharacterID = 3,
                    CharacterType = "Cosmo",
                    DisplayName = "Cosmo the Curious",
                    Description = "Curious and playful! Cosmo loves learning new things about saving and spending! ⭐",
                    BasePath = "/characters/cosmo",
                    IsActive = true,
                    DisplayOrder = 3
                },
                new Character
                {
                    CharacterID = 4,
                    CharacterType = "Aura",
                    DisplayName = "Aura the Wise",
                    Description = "Wise and encouraging! Aura believes in you and celebrates every achievement! ✨",
                    BasePath = "/characters/aura",
                    IsActive = true,
                    DisplayOrder = 4
                }
            );
        }
    }
}