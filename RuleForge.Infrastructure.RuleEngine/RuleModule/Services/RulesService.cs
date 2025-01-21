using RuleForge.Infrastructure.RuleEngine.RuleModule.Models;

namespace RuleForge.Infrastructure.RuleEngine.RuleModule.Services
{
    public class RulesService(IRulesEngine rulesEngine)
    {
        private readonly IRulesEngine _rulesEngine = rulesEngine;

        public async Task<RuleCheckModel> CheckRuleAsync(string workflowName, dynamic[] inputs)
        {
            List<RuleResultTree> resultList = await _rulesEngine.ExecuteAllRulesAsync(workflowName, inputs);

            return HandleResult(resultList);
        }

        /// <summary>
        /// Execute Workflow All Success
        /// </summary>
        /// <param name="workflowName">Workflow Name</param>
        /// <param name="inputs">Inputs</param>
        /// <returns>True if Any Pass, False if None Pass</returns>
        //public async ValueTask<bool> ExecuteWorkflowAllSuccess(string workflowName, params object[] inputs) =>
        //     (await ExecuteWorkflowAsync(workflowName, inputs)).All(a => a.IsSuccess);

        public async ValueTask<bool> ExecuteWorkflowAllSuccess(string workflowName, params object[] inputs)
        {
            List<RuleResultTree> resultList = await ExecuteWorkflowAsync(workflowName, inputs);
            return resultList.All(a => a.IsSuccess);
        }

        public async Task<List<RuleResultTree>> ExecuteWorkflowAllSuccess2(string workflowName, params object[] inputs)
        {
            return await ExecuteWorkflowAsync(workflowName, inputs);
        }

        /// <summary>
        /// Execute Workflow Any Success
        /// </summary>
        /// <param name="workflowName">Workflow Name</param>
        /// <param name="inputs">Inputs</param>
        /// <returns>True if Any Pass, False if None Pass</returns>
        public async ValueTask<bool> ExecuteWorkflowAnySuccess(string workflowName, params object[] inputs) =>
             (await ExecuteWorkflowAsync(workflowName, inputs)).Any(a => a.IsSuccess);

        /// <summary>
        /// Execute Workflow
        /// </summary>
        /// <param name="workflowName">Workflow Name</param>
        /// <param name="inputs">Inputs</param>
        /// <returns>List of Rule Result Tree</returns>
        private ValueTask<List<RuleResultTree>> ExecuteWorkflowAsync(string workflowName, params object[] inputs) =>
            _rulesEngine.ExecuteAllRulesAsync(workflowName, inputs);

        private static RuleCheckModel HandleResult(List<RuleResultTree> resultList)
        {
            List<string> errors = resultList
                .Where(result => !result.IsSuccess)
                .Select(result => result.ExceptionMessage)
                .ToList();

            return errors.Any()
                ? RuleCheckModel.CheckedOnFailure(errors)
                : RuleCheckModel.CheckedSuccessfully();
        }
    }
}