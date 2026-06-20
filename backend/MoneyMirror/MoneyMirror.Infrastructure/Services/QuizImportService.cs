using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Quiz;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoneyMirror.Infrastructure.Services
{
    public class QuizImportService : IQuizImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuizImportService> _logger;
        private readonly string _jsonFilePath;

        private static readonly Dictionary<string, int> PersonalityMap = new()
        {
            { "IMPULSIVE_SPENDER", 2 },
            { "PRUDENT_SAVER", 3 },
            { "GOAL_ORIENTED_PLANNER", 4 },
            { "BARGAIN_HUNTER", 5 }
        };

        public QuizImportService(
            ApplicationDbContext context,
            ILogger<QuizImportService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _jsonFilePath = configuration["QuizImport:JsonFilePath"]
                ?? throw new InvalidOperationException("QuizImport:JsonFilePath not configured");
        }

        public async Task<(bool success, QuizImportResultDto? result, string errorMessage)> ImportFromJsonAsync()
        {
            try
            {
                if (!File.Exists(_jsonFilePath))
                    return (false, null, $"JSON file not found at: {_jsonFilePath}");

                string json = await File.ReadAllTextAsync(_jsonFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var items = JsonSerializer.Deserialize<List<QuizJsonItem>>(json, options);

                if (items == null || items.Count == 0)
                    return (false, null, "JSON file contains no questions");

                var existingIds = (await _context.StoryQuizTemplates
                    .Select(s => s.StoryID)
                    .ToListAsync()).ToHashSet();

                var newItems = items.Where(i => !existingIds.Contains(i.Id)).ToList();

                if (newItems.Count == 0)
                {
                    return (true, new QuizImportResultDto
                    {
                        TotalInFile = items.Count,
                        NewlyImported = 0,
                        AlreadyExisted = items.Count
                    }, string.Empty);
                }

                int skippedInvalidPersonalities = 0;
                int importedCount = 0;

                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        // Step 1: Enable identity insert so we can supply StoryID explicitly
                        await _context.Database.ExecuteSqlRawAsync(
                            "SET IDENTITY_INSERT StoryQuizTemplates ON");

                        var answersToAdd = new List<QuizAnswer>();

                        foreach (var item in newItems)
                        {
                            if (item.Choices == null || item.Choices.Count != 4)
                            {
                                _logger.LogWarning(
                                    "Skipping StoryID {Id}: must have exactly 4 choices", item.Id);
                                continue;
                            }

                            // Step 2: Use raw SQL to insert the parent row.
                            // EF Core never includes identity column values in its generated
                            // INSERT statements, so we must bypass it here.
                            await _context.Database.ExecuteSqlRawAsync(
                                @"INSERT INTO StoryQuizTemplates
                                    (StoryID, QuestionText, TargetAgeMin, TargetAgeMax, CreatedDate)
                                  VALUES
                                    ({0}, {1}, {2}, {3}, {4})",
                                item.Id,
                                item.Story,
                                item.MinAge,
                                item.MaxAge,
                                DateTime.UtcNow);

                            importedCount++;

                            foreach (var choice in item.Choices)
                            {
                                if (!PersonalityMap.TryGetValue(choice.Personality, out int typeId))
                                {
                                    skippedInvalidPersonalities++;
                                    _logger.LogWarning(
                                        "Unknown personality '{Personality}' in StoryID {Id}",
                                        choice.Personality, item.Id);
                                    continue;
                                }

                                // Collect answers separately — parent must exist first
                                answersToAdd.Add(new QuizAnswer
                                {
                                    AnswerText = choice.Text,
                                    FeedbackMessage = choice.Feedback,
                                    StoryID = item.Id,
                                    PersonalityTypeID = typeId
                                });
                            }
                        }

                        // Step 3: Disable identity insert before EF Core takes over
                        await _context.Database.ExecuteSqlRawAsync(
                            "SET IDENTITY_INSERT StoryQuizTemplates OFF");

                        // Step 4: All parent rows are now committed — safe to insert children
                        if (answersToAdd.Count > 0)
                        {
                            _context.QuizAnswers.AddRange(answersToAdd);
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                _logger.LogInformation(
                    "Quiz import complete. Imported {Count} new questions.", importedCount);

                return (true, new QuizImportResultDto
                {
                    TotalInFile = items.Count,
                    NewlyImported = importedCount,
                    AlreadyExisted = items.Count - newItems.Count,
                    SkippedInvalidPersonalities = skippedInvalidPersonalities
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing quiz questions");
                return (false, null, $"Import failed: {ex.Message}");
            }
        }

        private class QuizJsonItem
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("min_age")]
            public int MinAge { get; set; }

            [JsonPropertyName("max_age")]
            public int MaxAge { get; set; }

            [JsonPropertyName("story")]
            public string Story { get; set; } = string.Empty;

            [JsonPropertyName("choices")]
            public List<QuizJsonChoice> Choices { get; set; } = new();
        }

        private class QuizJsonChoice
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;

            [JsonPropertyName("personality")]
            public string Personality { get; set; } = string.Empty;

            [JsonPropertyName("feedback")]
            public string Feedback { get; set; } = string.Empty;
        }
    }
}