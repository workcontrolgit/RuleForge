using RuleForge.Domain.Entities;
using RuleForge.Infrastructure.RuleEngine.RuleModule.Models;
using RuleForge.Infrastructure.RuleEngine.RuleModule.Services;
using RuleForge.WebApi.DTOs;

namespace RuleForge.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController(RulesService rulesService) : ControllerBase
    {
        private readonly RulesService _rulesService = rulesService;

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferRequestDto dto)
        {
            BankAccount account = new();
            dynamic[] inputs = [account, dto];
            RuleCheckModel ruleCheckModel = await _rulesService.CheckRuleAsync("Transfer", inputs);

            return (ruleCheckModel.IsSuccess) ? Ok() : BadRequest(ruleCheckModel.Errors);
        }
    }
}