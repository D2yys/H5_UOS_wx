using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AssetStreaming
{
    public class AddressableConfigController
    {
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();
        public static ParametersAddressableConfig pac = ParametersAddressableConfig.GetParametersAddressableConfig();

        public static string GetRemoteLoadPath(string badgeName)
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return "";
            }

            if (string.IsNullOrEmpty(badgeName) || UosCdnSettings.Settings.useLatest)
            {
                badgeName = "latest";
            }

            string host = Parameters.proxyHost[Parameters.backend];
            string remoteLoadPath = host + "client_api/v1/buckets/" + pb.selectedBucketUuid + "/release_by_badge/" + badgeName + "/entry_by_path/content/?path=";
            return remoteLoadPath;
        }
    }
}