using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedLookupTablesTracked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Categories
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories])
        BEGIN
            SET IDENTITY_INSERT [ExpenseCategories] ON;
            INSERT INTO [ExpenseCategories] ([CategoryID], [Name]) VALUES 
            (1, 'Snacks / Food'), (2, 'Games / Toys'), (3, 'Gifts'), (4, 'School Supplies'), (5, 'Other');
            SET IDENTITY_INSERT [ExpenseCategories] OFF;
        END");

            // 2. Characters
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM [Characters])
        BEGIN
            SET IDENTITY_INSERT [Characters] ON;
            INSERT INTO [Characters] ([CharacterID], [DefaultImageUrl], [Description], [Name]) VALUES
            (1, '/images/characters/nova/profile.png', 'Cool astronaut who loves street style and music', 'Nova'),
            (2, '/images/characters/cosmo/profile.png', 'Ninja superhero astronaut always ready for action', 'Cosmo'),
            (3, '/images/characters/luna/profile.png', 'Graceful ballerina astronaut in a pink skirt', 'Luna'),
            (4, '/images/characters/stella/profile.png', 'Laid back astronaut in a hoodie who loves bubblegum', 'Stella');
            SET IDENTITY_INSERT [Characters] OFF;
        END");

            // 3. Moods
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM [Moods])
        BEGIN
            SET IDENTITY_INSERT [Moods] ON;
            INSERT INTO [Moods] ([MoodID], [Description]) VALUES
            (1, 'Happy'), (2, 'Sad'), (3, 'Neutral'), (4, 'Excited'), (5, 'Regretful'), (6, 'Cool'), (7, 'Thoughtful');
            SET IDENTITY_INSERT [Moods] OFF;
        END");

            // 4. Character States
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM [CharacterStates])
        BEGIN
            SET IDENTITY_INSERT [CharacterStates] ON;
            INSERT INTO [CharacterStates] ([StateID], [CharacterID], [ImageUrl], [Message], [ScreenContext]) VALUES
            (6, 1, '/images/characters/nova/expenses.png', 'Let''s see what you copped this week, astronaut.', 'Expenses'),
            (7, 2, '/images/characters/cosmo/expenses.png', 'Time to investigate your spending like a true hero.', 'Expenses'),
            (8, 3, '/images/characters/luna/expenses.png', 'Every purchase tells a story. Let''s review yours.', 'Expenses'),
            (9, 4, '/images/characters/stella/expenses.png', 'No stress, let''s just vibe and see what you bought.', 'Expenses'),
            (10, 1, '/images/characters/nova/goals.png', 'Your bank account looking real nice right now.', 'Goals'),
            (11, 2, '/images/characters/cosmo/goals.png', 'Your savings power is charging up fast.', 'Goals'),
            (12, 3, '/images/characters/luna/goals.png', 'Your savings are dancing beautifully toward your dreams.', 'Goals'),
            (14, 4, '/images/characters/stella/goals.png', 'Pretty cool how your money''s stacking up like that.', 'Goals'),
            (15, 1, '/images/characters/nova/profile.png', 'Yo, what''s good? Time to check those space credits.', 'Profile'),
            (16, 2, '/images/characters/cosmo/profile.png', 'Looking strong, money warrior. Keep training.', 'Profile'),
            (17, 3, '/images/characters/luna/profile.png', 'Welcome back, little star. Shall we begin?', 'Profile'),
            (18, 4, '/images/characters/stella/profile.png', 'Hey there, space buddy. Just chilling and checking in.', 'Profile'),
            (19, 1, '/images/characters/nova/badges.png', 'Another badge? You''re on fire with this.', 'Badges'),
            (20, 2, '/images/characters/cosmo/badges.png', 'Another victory unlocked. You''re unstoppable.', 'Badges'),
            (23, 3, '/images/characters/luna/badges.png', 'Each achievement is like a perfect spin.', 'Badges'),
            (29, 4, '/images/characters/stella/badges.png', 'Nice, another one. You''re doing your thing.', 'Badges'),
            (32, 1, '/images/characters/nova/quiz.png', 'Aight, let''s test that money brain of yours.', 'Quiz'),
            (33, 2, '/images/characters/cosmo/quiz.png', 'Think fast, space cadet. Show me your skills.', 'Quiz'),
            (34, 3, '/images/characters/luna/quiz.png', 'Let''s gracefully glide through these questions together.', 'Quiz'),
            (35, 4, '/images/characters/stella/quiz.png', 'Take it easy, no rush. You got this.', 'Quiz');
            SET IDENTITY_INSERT [CharacterStates] OFF;
        END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Leave empty to protect your existing structural data
        }
    }
}