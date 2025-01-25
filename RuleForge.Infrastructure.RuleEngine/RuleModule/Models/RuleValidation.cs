namespace RuleForge.Infrastructure.RuleEngine.RuleModule.Models
{
    public class RuleValidation
    {
        public bool IsSuccess { get; set; }
        public List<string>? Errors { get; set; }

        public static RuleValidation CheckedSuccessfully() => new() { IsSuccess = true };

        public static RuleValidation CheckedOnFailure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
    }
}