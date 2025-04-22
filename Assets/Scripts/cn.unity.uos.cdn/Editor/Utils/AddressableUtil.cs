using System;
using System.IO;
using UnityEditor;
using UnityEngine;
#if ADDRESSABLES_EXISTS
using UnityEditor.AddressableAssets;
#endif

namespace AssetStreaming
{
    public class AddressableUtil
    {
        public static string getRemotebuildPath()
        {
#if ADDRESSABLES_EXISTS
            if (AddressableAssetSettingsDefaultObject.Settings == null)
            {
                return "";
            }
            string profileId = AddressableAssetSettingsDefaultObject.Settings.activeProfileId;
            string pathPattern = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(profileId, UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.kRemoteBuildPath);
            return pathPattern.Replace("[BuildTarget]", UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
#else
            return "";
#endif
        }
    }
}

