namespace RuleForge.Infrastructure.RuleEngine
{
    public static class ServiceRegistration
    {
        private static readonly string RULES_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Rules");

        public static void AddRuleSystem(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                string[] ruleFiles = RuleFileReader.GetRuleFiles(RULES_FOLDER_PATH);
                List<Workflow>? workflows = ruleFiles.SelectMany(filePath => RuleFileReader.LoadWorkflowsFromFile(filePath) ?? []).ToList();
                ReSettings reSettingsWithCustomTypes = new()
                {
                    CustomTypes = [typeof(CustomMethods)]
                };
                IRulesEngine ruleEngine = new RulesEngine.RulesEngine([.. workflows], reSettingsWithCustomTypes);

                return new RulesService(ruleEngine);
            });
        }
    }
}