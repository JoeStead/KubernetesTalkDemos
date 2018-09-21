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
            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://zordon.testcluster.vq.k8s"),
                Timeout = TimeSpan.FromMinutes(10)
            };
        }

        public async Task<DeployedInstance> DeployApplication()
        {
            var request = new CreateClusterModel("http://testcluster.vq.k8s:8001/", "CM", "SwanseaDemo", "latest");
            var response = await this.httpClient.PostAsJsonAsync("/deployments", request);

            var success = response.IsSuccessStatusCode;
            var deployedLocationId = response.Headers.Location.ToString().Split("/").LastOrDefault();

            var uriSuccess = int.TryParse(deployedLocationId, out var deployedId);

            if (!success)
            {
                if (deployedLocationId != null && uriSuccess)
                {
                    return new DeployedInstance(deployedId, null, null, false);
                }

                throw new Exception("Could not find deployed Id in location header");
            }

            var locationHeader = new Uri($"http://{response.Headers.Location}");

            var clusterLocation = await this.httpClient.GetAsync(locationHeader.PathAndQuery);

            success = clusterLocation.IsSuccessStatusCode;
            if (!success)
            {
                return new DeployedInstance(deployedId, null, null, false);
            }

            var data = JsonConvert.DeserializeObject<ClusterCreationResult>(await clusterLocation.Content.ReadAsStringAsync());
            var deployedUri = new Uri(data.DeployedUri);

            var loginUriBuilder = new UriBuilder(deployedUri) { Host = "login." + deployedUri.Host };

            return new DeployedInstance(deployedId, deployedUri, loginUriBuilder.Uri, data.Deployed);
        }

        public async Task WaitForInstanceToBeHealthy(Uri address)
        {
            var startTime = DateTime.UtcNow;
            while (!await this.CheckHealthOfDeployedInstance(address))
            {
                if (DateTime.UtcNow > startTime.AddSeconds(120))
                {
                    throw new Exception("CM did not come online after 120 seconds continue to next test on");
                }

                await Task.Delay(1000);
            }
        }

        private async Task<bool> CheckHealthOfDeployedInstance(Uri address)
        {
            var client = new HttpClient
            {
                BaseAddress = address
            };
            try
            {
                var response = await client.GetAsync("health");
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public Task<HttpResponseMessage> DeleteDeployment(int id)
        {
            return this.httpClient.DeleteAsync($"/deployments/{id}");
        }
    }
}
