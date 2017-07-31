using ManagementClient.Helpers;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementClient
{
    class Program
    {
        private static Policy _policy;

        static void Main(string[] args)
        {            
            RunAsync(new SettingsHelper()).GetAwaiter().GetResult();
        }

        static async Task RunAsync(ISettingsHelper settings)
        {
            var credential = await AuthenticationHelper.GetTokenCredential(
                settings.ClientId,
                settings.ClientSecret,
                settings.Tenant);
            
            var resourceManagementClient = new ResourceManagementClient(credential)
            {
                SubscriptionId = settings.SubscriptionId,
                LongRunningOperationRetryTimeout = 60
            };

            var networkManagementClient = new NetworkManagementClient(credential)
            {
                SubscriptionId = settings.SubscriptionId,
                LongRunningOperationRetryTimeout = 60
            };
            var computeManagementClient = new ComputeManagementClient(credential)
            {
                SubscriptionId = settings.SubscriptionId,
                LongRunningOperationRetryTimeout = 60
            };

            _policy = Polly429RetryPolicy.SetRetryPolicy();

            var resourceGroup = await _policy.ExecuteAsync(async () => 
                await CreateResourceGroupAsync(
                    resourceManagementClient, 
                    settings.ResourceGroupName, 
                    settings.Location));

            var vnet = await _policy.ExecuteAsync(async () =>
                await CreateVirtualNetworkAsync(
                    settings.VirtualNetworkName,
                    networkManagementClient,
                    settings.ResourceGroupName,
                    settings.Location));
        }

        public static async Task<ResourceGroup> CreateResourceGroupAsync(
            ResourceManagementClient resourceManagementClient,
            string groupName,
            string location)
        {
            var resourceGroup = new ResourceGroup { Location = location };
            return await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(
              groupName,
              resourceGroup);
        }

        public static async Task<VirtualNetwork> CreateVirtualNetworkAsync(
            string name,
            NetworkManagementClient networkManagementClient,
            string groupName,
            string location)
        {
            var subnet = new Subnet
            {
                Name = "default",
                AddressPrefix = "10.0.0.0/24"
            };
            var address = new AddressSpace
            {
                AddressPrefixes = new List<string> { "10.0.0.0/16" }
            };
            var ret = await networkManagementClient.VirtualNetworks.CreateOrUpdateAsync(
              groupName,
              name,
              new VirtualNetwork
              {
                  Location = location,
                  AddressSpace = address,
                  Subnets = new List<Subnet> { subnet }
              });


            return ret;
        }


    }
}
