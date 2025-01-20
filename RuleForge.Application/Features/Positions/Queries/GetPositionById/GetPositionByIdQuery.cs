using MediatR;
using RuleForge.Application.DTOs;
using RuleForge.Infrastructure.RuleEngine.RuleModule.Models;
using RuleForge.Infrastructure.RuleEngine.RuleModule.Services;
using RulesEngine;
using RulesEngine.Models;
using System.Security.Principal;
using static FastExpressionCompiler.ExpressionCompiler;

namespace RuleForge.Application.Features.Positions.Queries.GetPositionById
{
    // Represents a query to get a position by its ID
    public class GetPositionByIdQuery : IRequest<Response<Position>>
    {
        // The ID of the position to retrieve
        public Guid Id { get; set; }

        // Handles the GetPositionByIdQuery request and retrieves the corresponding Position entity from the repository.
        public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, Response<Position>>
        {
            private readonly IPositionRepositoryAsync _repository;

            private readonly RulesService _rulesService;

            // Constructor that initializes the repository
            public GetPositionByIdQueryHandler(IPositionRepositoryAsync repository, RulesService rulesService)
            {
                _repository = repository;
                _rulesService = rulesService;
            }

            // Handles the request and returns a response containing the Position entity if it exists, otherwise throws an ApiException.
            public async Task<Response<Position>> Handle(GetPositionByIdQuery query, CancellationToken cancellationToken)
            {
                // Create a MediatR request
                var request = new EvaluateRulesRequest
                {
                    InputData = new { TotalAmount = 150 },
                    WorkflowName = "PositionWorkflow"
                };
                // Example input
                var input1 = request.InputData;
                var input2 = request.WorkflowName; ;

                dynamic[] inputs = [input1, input2];

                RuleCheckModel ruleCheckModel = await _rulesService.CheckRuleAsync(request.WorkflowName, inputs);

                var workflowAllSuccess = await _rulesService.ExecuteWorkflowAllSuccess(request.WorkflowName, inputs);
                var workflowAnySuccess = await _rulesService.ExecuteWorkflowAnySuccess(request.WorkflowName, inputs);

                // Create RuleParameter
                var input2s = new[] { new RuleParameter("basicInfo", input1), new RuleParameter("orderInfo", input2) };

                //var rp1 = new RuleParameter("basicInfo", input1);
                //var rp2 = new RuleParameter("orderInfo", input2);
                var workflowAllSuccess2 = await _rulesService.ExecuteWorkflowAllSuccess("PositionWorkflow2", input2s);

                var entity = await _repository.GetByIdAsync(query.Id);
                if (entity == null) throw new ApiException($"Position Not Found.");
                return new Response<Position>(entity);
            }
        }
    }
}