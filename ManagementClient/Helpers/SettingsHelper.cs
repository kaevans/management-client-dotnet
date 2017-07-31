using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementClient.Helpers
{
    internal class SettingsHelper : ISettingsHelper
    {
        private string _randomName;
        internal SettingsHelper()
        {
            _randomName =  Guid.NewGuid().ToString().Substring(0, 8).ToLower();            
        }

        public string ResourceGroupName { get { return "rg" + _randomName; } }
        public string VirtualNetworkName { get { return "vnet" + _randomName; } }
        public string NetworkInterfaceName { get { return "nic" + _randomName; } }
        public string VirtualMachineName { get { return "vm" + _randomName; } }
        public string SubscriptionId { get { return ConfigurationManager.AppSettings["subscriptionId"]; } }
        public string Location { get { return ConfigurationManager.AppSettings["location"]; } }
        public string ClientId { get { return ConfigurationManager.AppSettings["ida:clientId"]; } }
        public string ClientSecret { get { return ConfigurationManager.AppSettings["ida:clientSecret"]; } }
        public string Tenant { get { return ConfigurationManager.AppSettings["ida:tenant"]; } }
    }
}
