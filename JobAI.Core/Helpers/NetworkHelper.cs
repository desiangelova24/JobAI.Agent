using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Core.Helpers
{
    public static class NetworkHelper
    {
        public static bool IsInternetAvailable()
        {
            try
            {
                using (var client = new System.Net.NetworkInformation.Ping())
                {
                    var reply = client.Send("8.8.8.8", 3000);
                    return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch { return false; }
        }
    }
}
