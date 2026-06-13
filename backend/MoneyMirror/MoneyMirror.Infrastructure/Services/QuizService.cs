using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Quiz;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuizService> _logger;

        private const int DailyQuizLimit = 2;
        private const int WeeklySummaryCount = 7;

        private readonly IAchievementService _achievementService;

        public QuizService(ApplicationDbContext context, ILogger<QuizService> logger, IAchievementService achievementService)
        {
            _context = context;
            _logger = logger;
            _achievementService = achievementService;
        }

        public async Task<(bool success, NextQuizQuestionDto? question, string message)>
            GetNextQuestionAsync(int childId)
        {
            try
            {
                var child = await _context.Children.FindAsync(childId);
                if (child == null)
                    return (false, null, "Child not found");

                // Check daily limit
                var todayUtc = DateTime.UtcNow.Date;
                int todayCount = await _context.QuizLogs
                    .CountAsync(q => q.ChildID == childId && q.CompletedDate.Date == todayUtc);

                if (todayCount >= DailyQuizLimit)
                    return (true, null, "Come back tomorrow!");

                // Find the last answered StoryID for this child
                var lastAnsweredStoryId = await _context.QuizLogs
                    .Where(q => q.ChildID == childId)
                    .OrderByDescending(q => q.StoryID)
                    .Select(q => (int?)q.StoryID)
                    .FirstOrDefaultAsync();

                // Get IDs of all stories this child already answered
                var answeredStoryIds = await _context.QuizLogs
                    .Where(q => q.ChildID == childId)
                    .Select(q => q.StoryID)
                    .Distinct()
                    .ToListAsync();

                // Find next question: sequential by ID, age-appropriate, not yet answered
                var nextQuestion = await _context.StoryQuizTemplates
                    .Include(s => s.Answers)
                    .Where(s =>
                        s.TargetAgeMin <= child.Age &&
                        s.TargetAgeMax >= child.Age &&
                        !answeredStoryIds.Contains(s.StoryID) &&
                        (lastAnsweredStoryId == null || s.StoryID > lastAnsweredStoryId))
                    .OrderBy(s => s.StoryID)
                    .FirstOrDefaultAsync();

                if (nextQuestion == null)
                    return (true, null, "You've completed all available quizzes! More coming soon.");

                var dto = new NextQuizQuestionDto
                {
                    StoryID = nextQuestion.StoryID,
                    Title = nextQuestion.Title,
                    QuestionText = nextQuestion.QuestionText,
                    Answers = nextQuestion.Answers.Select(a => new QuizAnswerOptionDto
                    {
                        AnswerID = a.AnswerID,
                        AnswerText = a.AnswerText
                    }).ToList()
                };

                return (true, dto, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next quiz question for child {ChildId}", childId);
                return (false, null, "An error occurred while loading the quiz");
            }
        }

        public async Task<(bool success, SubmitQuizAnswerResponseDto? response, string errorMessage)>
    SubmitAnswerAsync(int childId, SubmitQuizAnswerDto dto)
        {
            try
            {
                var child = await _context.Children.FindAsync(childId);
                if (child == null)
                    return (false, null, "Child not found");

                var answer = await _context.QuizAnswers
                    .Include(a => a.StoryQuizTemplate)
                    .FirstOrDefaultAsync(a => a.AnswerID == dto.AnswerID);

                if (answer == null)
                    return (false, null, "Invalid answer");

                bool alreadyAnswered = await _context.QuizLogs
                    .AnyAsync(q => q.ChildID == childId && q.StoryID == answer.StoryID);

                if (alreadyAnswered)
                    return (false, null, "You already answered this question");

                var log = new Core.Models.QuizLog
                {
                    ChildID = childId,
                    StoryID = answer.StoryID,
                    AnswerID = dto.AnswerID,
                    CompletedDate = DateTime.UtcNow
                };

                _context.QuizLogs.Add(log);

                child.QuizCount++;
                _context.Children.Update(child);

                await _context.SaveChangesAsync();

                await _achievementService.CheckAndUnlockAsync(childId, "Quiz");

                _logger.LogInformation(
                    "Child {ChildId} answered StoryID {StoryId} with AnswerID {AnswerId}",
                    childId, answer.StoryID, dto.AnswerID);

                return (true, new SubmitQuizAnswerResponseDto
                {
                    FeedbackMessage = answer.FeedbackMessage
                }, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz answer for child {ChildId}", childId);
                return (false, null, "An error occurred while submitting your answer");
            }
        }

        //public async Task<List<ChildQuizSummaryDto>> GetWeeklyQuizSummariesAsync()
        //{
        //    try
        //    {
        //        // Personality type IDs - these must match DB seed data
        //        // Expected: 2=Impulsive, 3=Prudent Saver, 4=Goal-Oriented Planner, 5=Bargain Hunter
        //        // (1 is Pending Analysis)
        //        // We group by the PersonalityTypeID on the chosen answer

        //        var childIds = await _context.Children
        //            .Select(c => c.ChildID)
        //            .ToListAsync();

        //        var summaries = new List<ChildQuizSummaryDto>();

        //        foreach (var childId in childIds)
        //        {
        //            var last7 = await _context.QuizLogs
        //                .Include(q => q.QuizAnswer)
        //                    .ThenInclude(a => a.PersonalityType)
        //                .Where(q => q.ChildID == childId)
        //                .OrderByDescending(q => q.CompletedDate)
        //                .Take(WeeklySummaryCount)
        //                .ToListAsync();

        //            if (!last7.Any()) continue;

        //            var summary = new ChildQuizSummaryDto { ChildID = childId };

        //            foreach (var log in last7)
        //            {
        //                var typeName = log.QuizAnswer?.PersonalityType?.ParentName;
        //                switch (typeName)
        //                {
        //                    case "Impulsive Spender": summary.Impulsive++; break;
        //                    case "Prudent Saver": summary.Saver++; break;
        //                    case "Goal-Oriented Planner": summary.Planner++; break;
        //                    case "Bargain Hunter": summary.Bargain++; break;
        //                }
        //            }

        //            summaries.Add(summary);
        //        }

        //        return summaries;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error generating weekly quiz summaries");
        //        return new List<ChildQuizSummaryDto>();
        //    }
        //}
    }
}