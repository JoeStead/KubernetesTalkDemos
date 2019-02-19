namespace AutomatedInfrastructureTests.KubernetesDeployers
{
    class CreateClusterModel
    {
        public string ClusterApiUrl { get; }

        public string DeploymentSet { get; }

        public string DeploymentTag { get; }

        public string DockerTag { get;  }

        public CreateClusterModel(string clusterApiUrl, string deploymentSet, string deploymentTag, string dockerTag)
        {
            ClusterApiUrl = clusterApiUrl;
            DeploymentSet = deploymentSet;
            DeploymentTag = deploymentTag;
            DockerTag = dockerTag;
        }
    }
}
