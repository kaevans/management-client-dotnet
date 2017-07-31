using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementClient.Helpers
{
    interface ISettingsHelper
    {
        string ResourceGroupName { get; }
        string VirtualNetworkName { get; }
        string NetworkInterfaceName { get; }
        string VirtualMachineName { get; }
        string SubscriptionId { get; }
        string Location { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Tenant { get; }
    }
}
