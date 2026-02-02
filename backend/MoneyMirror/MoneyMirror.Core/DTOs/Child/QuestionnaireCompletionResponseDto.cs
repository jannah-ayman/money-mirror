namespace MoneyMirror.Core.DTOs.Child
{
    public class QuestionnaireCompletionResponseDto
    {
        public string ChildLoginCode { get; set; }

        public int ChildID { get; set; }
        public string ChildFirstName { get; set; }

        public PersonalityProfileDto PersonalityProfile { get; set; }
        public bool IsPersonalityFinalized { get; set; }
    }
    public class PersonalityProfileDto
    {
        public int TypeID { get; set; }

        public string ParentName { get; set; }

        public string ChildName { get; set; }
        public string Description { get; set; }
        public string? FunFacts { get; set; }
    }
}
