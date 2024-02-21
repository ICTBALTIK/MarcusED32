using Smart.Network.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAuto.Utils
{
    public class GravityWepApiClient : WebApiBase<GravityWepApiClient>
    {
        //string _urlGravityApi = "http://172.23.1.139/GravityApi/api/"; //sul 172.23.0.100
        string _urlGravityApi = "http://172.23.0.100/GravityApi/api/"; //sul 172.23.0.100

        public override void Setup(string webApiUrl = null)
        {
            base.Setup(webApiUrl);
            base.Client.Timeout = TimeSpan.FromSeconds(200);
        }
        public void Setup()
        {
            string url = _urlGravityApi;
            Setup(url);
        }
        public GravityWepApiClient Parameter() { base.SetController("Parameter"); return this; }
        public GravityWepApiClient Production() { base.SetController("Production"); return this; }
        public GravityWepApiClient GravityFile() { base.SetController("GravityFile"); return this; }
        public GravityWepApiClient Runtime() { base.SetController("Runtime"); return this; }
        public GravityWepApiClient Utility() { base.SetController("Utility"); return this; }
        public GravityWepApiClient Tracking() { base.SetController("Tracking"); return this; }
        public GravityWepApiClient Users() { base.SetController("Users"); return this; }

    }
}