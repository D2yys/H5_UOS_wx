using System;
using System.Text;
using System.Threading.Tasks;
using Entitlement;
using Guild;
using Google.Protobuf;
using Unity.UOS.Common;
using Unity.UOS.Networking;

namespace Unity.Passport.Sample.Scripts
{
    public class MockServer
    {
        private static MockServer sRuntimeInstance;
        private string _basicAuthToken;
        public static MockServer Instance
        {
            get
            {
                if (sRuntimeInstance != null)
                {
                    return sRuntimeInstance;
                }

                throw new NullReferenceException("Mock Server Not Initialized");
            }
        }

        public static void InitInstance()
        {
            if (sRuntimeInstance != null) return;
            sRuntimeInstance = new MockServer();

#if UNITY_SERVER || UNITY_EDITOR
            sRuntimeInstance._basicAuthToken = "Basic " + System.Convert.ToBase64String(Encoding.GetEncoding(28591)
                .GetBytes(Settings.AppID + ":" + Settings.AppServiceSecret));
#endif
        }
    }
}