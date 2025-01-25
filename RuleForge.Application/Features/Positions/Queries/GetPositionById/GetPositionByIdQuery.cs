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
        public Guid Id { get; set; } = new Guid("3e2e28b2-cd36-483b-a018-1ef8f209aeba");

        // Handles the GetPositionByIdQuery request and retrieves the corresponding Position entity from the repository.
        public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, Response<Position>>
        {
            private readonly IPositionRepositoryAsync _repository;

            private readonly RulesService _rulesService;

            private Dictionary<string, object> Dimensions { get; set; }

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
                var request = new RuleDataRequest
                {
                    InputData = new { TotalAmount = 150 },
                    WorkflowName = "PositionWorkflow"
                };
                // Example input
                var inputData = request.InputData;
                var workflowName = request.WorkflowName;

                dynamic[] inputs = { inputData, workflowName };

                RuleValidation ruleCheckModel = await _rulesService.ValidateRuleAsync(workflowName, inputs);

                var workflowAllSuccess = await _rulesService.ExecuteWorkflowAllSuccess(workflowName, inputs);
                var workflowAnySuccess = await _rulesService.ExecuteWorkflowAnySuccess(workflowName, inputs);

                var resultsPositionWorkflow = await _rulesService.ExecuteWorkflowAllSuccess2(workflowName, inputs);

                // Create RuleParameter
                //var input2s = new[] { new RuleParameter("basicInfo", input1), new RuleParameter("orderInfo", input2) };

                var input1 = new
                {
                    country = "india",
                    loyalityFactor = 1,
                    totalPurchasesToDate = 6000
                };

                var input2 = new
                {
                    totalOrders = 3
                };

                var input3 = new
                {
                    noOfVisitsPerMonth = 3
                };

                var rp1 = new RuleParameter("basicInfo", input1);
                var rp2 = new RuleParameter("orderInfo", input2);
                var rp3 = new RuleParameter("telemetryInfo", input3);

                //var rp1 = new RuleParameter("basicInfo", input1);
                //var rp2 = new RuleParameter("orderInfo", input2);
                // 3e2e28b2-cd36-483b-a018-1ef8f209aeba
                var workflowAllSuccess2 = await _rulesService.ExecuteWorkflowAllSuccess("DiscountWithCustomInputNames", rp1, rp2, rp3);

                var resultsDiscountWithCustomInputNames = await _rulesService.ExecuteWorkflowAllSuccess2("DiscountWithCustomInputNames", rp1, rp2, rp3);

                // Define input data (context)
                Dimensions = new Dictionary<string, object>
    {
        { "Age", 20 },
        { "PurchaseAmount", 150 }
    };

                // Execute rules
                var results = await _rulesService.ExecuteWorkflowAllSuccess2("RuleSet1", Dimensions);

                foreach (var result in results)
                {
                    Console.WriteLine($"Rule: {result.Rule.RuleName}, Success: {result.IsSuccess}");
                    if (result.IsSuccess && result.ActionResult != null)
                    {
                        ExecuteAction(result.ActionResult);
                    }
                }

                var rp = new RuleParameter[] { new("myValue", new { Value1 = "Fabrikam" }) };
                var resultsmy_workflow = await _rulesService.ExecuteWorkflowAllSuccess2("my_workflow", rp);

                var entity = await _repository.GetByIdAsync(query.Id);
                if (entity == null) throw new ApiException($"Position Not Found.");
                return new Response<Position>(entity);
            }

            // Action Execution
            private static void ExecuteAction(ActionResult actionResult)
            {
                switch (actionResult.ToString())
                {
                    case "GrantAccess":
                        GrantAccess();
                        break;

                    case "ApplyDiscount":
                        {
                            ApplyDiscount(Convert.ToInt32(10));
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown action.");
                        break;
                }
            }

            // Action Methods
            private static void GrantAccess()
            {
                Console.WriteLine("Access Granted!");
            }

            private static void ApplyDiscount(int discountPercentage)
            {
                Console.WriteLine($"Discount of {discountPercentage}% applied.");
            }
        }
    }
}