namespace MoneyMirror.Core.DTOs.Quiz
{
    public class QuizImportResultDto
    {
        public int TotalInFile { get; set; }
        public int NewlyImported { get; set; }
        public int AlreadyExisted { get; set; }
        public int SkippedInvalidPersonalities { get; set; }
    }
}