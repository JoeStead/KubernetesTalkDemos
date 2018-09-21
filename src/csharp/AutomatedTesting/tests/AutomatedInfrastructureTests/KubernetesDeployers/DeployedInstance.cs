using System;

namespace AutomatedInfrastructureTests.KubernetesDeployers
{
    public class DeployedInstance
    {
        public int Id { get; }

        public Uri DeployedUri { get; }

        public Uri DeployedIdentityServerUri { get; set; }

        public bool Success { get; }

        public DeployedInstance(int id, Uri deployedUri, Uri deployedIdentityServerUri, bool success)
        {
            this.Id = id;
            this.DeployedUri = deployedUri;
            this.DeployedIdentityServerUri = deployedIdentityServerUri;
            this.Success = success;
        }
    }
}
