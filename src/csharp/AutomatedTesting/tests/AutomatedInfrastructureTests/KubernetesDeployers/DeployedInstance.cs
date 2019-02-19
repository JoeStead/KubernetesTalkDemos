using System;

namespace AutomatedInfrastructureTests.KubernetesDeployers
{
    public class DeployedInstance
    {
        public int Id { get; }

        public Uri DeployedUri { get; }

        public bool Success { get; }

        public DeployedInstance(int id, Uri deployedUri, bool success)
        {
            Id = id;
            DeployedUri = deployedUri;
            Success = success;
        }
    }
}
