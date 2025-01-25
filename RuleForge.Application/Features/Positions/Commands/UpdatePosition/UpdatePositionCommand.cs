using MediatR;
using RuleForge.Infrastructure.RuleEngine.RuleModule.Models;
using RuleForge.Infrastructure.RuleEngine.RuleModule.Services;
using System.Runtime.InteropServices;

namespace RuleForge.Application.Features.Positions.Commands.UpdatePosition
{
    // Define a command class for updating a position
    public class UpdatePositionCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; } // Unique identifier of the position to be updated
        public string PositionTitle { get; set; } // New title for the position
        public string PositionNumber { get; set; } // New number for the position (optional)
        public string PositionDescription { get; set; } // New description for the position
        public Guid DepartmentId { get; set; } // ID of the department to which the position belongs
        public Guid SalaryRangeId { get; set; } // ID of the salary range for the position

        // Define a handler class for processing the update command
        public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, Response<Guid>>
        {
            private readonly IPositionRepositoryAsync _repository; // Repository for accessing positions

            private readonly RulesService _rulesService;

            // Constructor to inject the repository
            public UpdatePositionCommandHandler(IPositionRepositoryAsync positionRepository, RulesService rulesService)
            {
                _repository = positionRepository;
                _rulesService = rulesService;
            }

            // Handle method to process the update command
            public async Task<Response<Guid>> Handle(UpdatePositionCommand command, CancellationToken cancellationToken)
            {
                dynamic[] inputs = [command];

                RuleCheckModel ruleCheckModel = await _rulesService.CheckRuleAsync("PositionWorkflow", inputs);

                var position = await _repository.GetByIdAsync(command.Id); // Get the position by ID

                if (position == null)
                {
                    throw new ApiException($"Position Not Found."); // Throw an exception if the position is not found
                }
                else
                {
                    // Update the position with the new values from the command
                    UpdatePosition(command, position);

                    await _repository.UpdateAsync(position); // Save the updated position to the repository

                    return new Response<Guid>(position.Id); // Return a response containing the ID of the updated position
                }

                void UpdatePosition(UpdatePositionCommand command, Position position)
                {
                    position.PositionNumber = command.PositionNumber;
                    position.PositionTitle = command.PositionTitle;
                    position.PositionDescription = command.PositionDescription;
                    position.DepartmentId = command.DepartmentId;
                    position.SalaryRangeId = command.SalaryRangeId;
                }
            }
        }
    }
}