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
            this.ClusterApiUrl = clusterApiUrl;
            this.DeploymentSet = deploymentSet;
            this.DeploymentTag = deploymentTag;
            this.DockerTag = dockerTag;
        }
    }
}
