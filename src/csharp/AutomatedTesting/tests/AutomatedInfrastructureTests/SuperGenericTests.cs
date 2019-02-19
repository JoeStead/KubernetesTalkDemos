using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutomatedInfrastructureTests.KubernetesDeployers;
using Xunit;

namespace AutomatedInfrastructureTests
{
    public class SuperGenericTests
    {
        [Fact]
        public async Task Unauthorised_User_Receives_401_Response()
        {
            var deployer = new KubernetesDeployer();

            var deployment = await deployer.DeployApplication();

            await deployer.WaitForInstanceToBeHealthy(deployment.DeployedUri);

            var apiClient = new HttpClient();

            var response = await apiClient.GetAsync($"{deployment.DeployedUri}/secrets");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            await deployer.DeleteDeployment(deployment.Id);
        }
    }
}
