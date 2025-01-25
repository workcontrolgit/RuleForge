using MediatR;

namespace RuleForge.Application.DTOs
{
    public class RuleDataRequest : RuleDataResponse
    {
        public object InputData { get; set; }
        public string WorkflowName { get; set; }
    }
}