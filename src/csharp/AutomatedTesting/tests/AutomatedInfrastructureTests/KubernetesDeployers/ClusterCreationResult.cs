namespace AutomatedInfrastructureTests.KubernetesDeployers
{
    internal class ClusterCreationResult
    {
        public int Id { get; set; }

        public string DeployedUri { get; set; }

        public bool Deployed { get; set; }

    }
}
