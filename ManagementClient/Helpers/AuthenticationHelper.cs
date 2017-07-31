using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementClient.Helpers
{
    static class AuthenticationHelper
    {
        internal static async Task<TokenCredentials> GetTokenCredential(
            string clientId,
            string clientSecret,
            string tenant)
        {
            var credential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenant));
            var result = await context.AcquireTokenAsync("https://management.azure.com/", credential);
            if (result == null)
            {
                throw new InvalidOperationException("Could not get the token");
            }
            else
            {
                return new TokenCredentials(result.AccessToken);
            }            
        }
    }
}
