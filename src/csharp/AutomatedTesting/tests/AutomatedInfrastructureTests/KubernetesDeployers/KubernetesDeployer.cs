using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AutomatedInfrastructureTests.KubernetesDeployers
{
    public class KubernetesDeployer
    {

        private readonly HttpClient httpClient;

        public KubernetesDeployer()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:6969"),
                Timeout = TimeSpan.FromMinutes(10)
            };
        }

        public async Task<DeployedInstance> DeployApplication()
        {
            throw new Exception("Go get the cluster IP idiot");
            var request = new CreateClusterModel("http://127.0.0.1:33071", "demo-app", "NL-Demo", "latest");
            var response = await httpClient.PostAsJsonAsync("/deployments", request);

            var success = response.IsSuccessStatusCode;
            var deployedLocationId = response.Headers.Location.ToString().Split("/").LastOrDefault();

            var uriSuccess = int.TryParse(deployedLocationId, out var deployedId);

            if (!success)
            {
                if (deployedLocationId != null && uriSuccess)
                {
                    return new DeployedInstance(deployedId, null, false);
                }

                throw new Exception("Could not find deployed Id in location header");
            }

            var locationHeader = new Uri($"http://{response.Headers.Location}");

            var clusterLocation = await httpClient.GetAsync(locationHeader.PathAndQuery);

            success = clusterLocation.IsSuccessStatusCode;
            if (!success)
            {
                return new DeployedInstance(deployedId, null, false);
            }

            var data = JsonConvert.DeserializeObject<ClusterCreationResult>(await clusterLocation.Content.ReadAsStringAsync());
            var deployedUri = new Uri($"http://192.168.99.103/{data.DeployedUri}");


            return new DeployedInstance(deployedId, deployedUri, data.Deployed);
        }

        public async Task WaitForInstanceToBeHealthy(Uri address)
        {
            var startTime = DateTime.UtcNow;
            while (!await CheckHealthOfDeployedInstance(address))
            {
                if (DateTime.UtcNow > startTime.AddSeconds(120))
                {
                    throw new Exception("Demo App did not come online after 120 seconds continue to next test on");
                }

                await Task.Delay(1000);
            }
        }

        private async Task<bool> CheckHealthOfDeployedInstance(Uri address)
        {
            var client = new HttpClient();
            try
            {
                var response = await client.GetAsync($"{address}/health");
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public Task<HttpResponseMessage> DeleteDeployment(int id)
        {
            return httpClient.DeleteAsync($"/deployments/{id}");
        }
    }
}
