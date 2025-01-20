using MediatR;

namespace RuleForge.Application.DTOs
{
    public class EvaluateRulesRequest : IRequest<EvaluateRulesResponse>
    {
        public object InputData { get; set; }
        public string WorkflowName { get; set; }
    }
}