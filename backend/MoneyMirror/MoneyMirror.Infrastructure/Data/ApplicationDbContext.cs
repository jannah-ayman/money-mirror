using Microsoft.EntityFrameworkCore;
using MoneyMirror.Core.Models;

namespace MoneyMirror.Infrastructure.Data
{
    /// <summary>
    /// Main database context for Money Mirror application.
    /// Manages all entities and their relationships in SQL Server.
    /// Configured for use with Entity Framework Core 8.0 and SQL Server 2019+.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor that receives database configuration from dependency injection.
        /// Called automatically by ASP.NET Core when DbContext is requested.
        /// </summary>
        /// <param name="options">Configuration options including connection string</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==================== DbSets (Tables) ====================
        // Each DbSet<T> represents a table in the database
        // Table name matches the property name by convention

        /// <summary>
        /// Parents table - stores parent/guardian accounts with authentication
        /// </summary>
        public DbSet<Parent> Parents { get; set; }

        /// <summary>
        /// Children table - stores child user accounts
        /// </summary>
        public DbSet<Child> Children { get; set; }

        /// <summary>
        /// ParentChild junction table - many-to-many relationship between parents and children
        /// Allows one parent to manage multiple children, and one child to have multiple parents
        /// </summary>
        public DbSet<ParentChild> ParentChildren { get; set; }

        /// <summary>
        /// Expenses table - logs of purchases made by children
        /// </summary>
        public DbSet<Expense> Expenses { get; set; }

        /// <summary>
        /// ExpenseCategory table - predefined categories for classifying expenses
        /// </summary>
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        /// <summary>
        /// SavingsGoals table - goals set by children or parents
        /// </summary>
        public DbSet<SavingsGoal> SavingsGoals { get; set; }

        /// <summary>
        /// Allowances table - recurring or one-time money given to children
        /// </summary>
        public DbSet<Allowance> Allowances { get; set; }

        /// <summary>
        /// Notifications table - alerts sent to parents and children
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// PersonalityTypes table - financial personality classifications
        /// </summary>
        public DbSet<PersonalityType> PersonalityTypes { get; set; }

        /// <summary>
        /// InitialProfilingQuestionnaire table - parent responses for child setup
        /// </summary>
        public DbSet<InitialProfilingQuestionnaire> InitialProfilingQuestionnaires { get; set; }

        /// <summary>
        /// QuizLogs table - child responses to story quizzes
        /// </summary>
        public DbSet<QuizLog> QuizLogs { get; set; }

        /// <summary>
        /// StoryQuizTemplate table - predefined story scenarios for quizzes
        /// </summary>
        public DbSet<StoryQuizTemplate> StoryQuizTemplates { get; set; }

        /// <summary>
        /// ChildAchievements junction table - tracks which achievements each child has earned
        /// </summary>
        public DbSet<ChildAchievement> ChildAchievements { get; set; }

        /// <summary>
        /// AchievementTypes table - predefined achievement/badge definitions
        /// </summary>
        public DbSet<AchievementType> AchievementTypes { get; set; }

        /// <summary>
        /// CharacterStats table - predefined character animation states
        /// </summary>
        public DbSet<CharacterStats> CharacterStats { get; set; }

        /// <summary>
        /// ChildCharacterStats table - logs of character reactions shown to children
        /// </summary>
        public DbSet<ChildCharacterStats> ChildCharacterStats { get; set; }

        /// <summary>
        /// Moods table - predefined mood emojis for expense logging
        /// </summary>
        public DbSet<Mood> Moods { get; set; }

        /// <summary>
        /// Advice table - AI-generated personalized financial tips
        /// </summary>
        public DbSet<Advice> Advices { get; set; }

        /// <summary>
        /// Configure entity relationships, indexes, constraints, and default values.
        /// Called automatically by Entity Framework when creating migrations.
        /// </summary>
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

                entity.Property(c => c.ProfileCompletionStatus)
                      .HasDefaultValue(false);

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Configure optional foreign key to PersonalityType
                entity.HasOne(c => c.PersonalityType)
                      .WithMany(pt => pt.Children)
                      .HasForeignKey(c => c.TypeID)
                      .OnDelete(DeleteBehavior.SetNull); // Don't delete child if personality type is removed
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