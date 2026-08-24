namespace SchurkoPortfolio.Core.Model
{
    public class AzureOpenAiModel
    {
        public string Endpoint { get; set; }
        public string DeploymentName { get; set; }
        public string ApiKey { get; set; }
        public string EmbeddingDeploymentName { get; set; }
    }
}
