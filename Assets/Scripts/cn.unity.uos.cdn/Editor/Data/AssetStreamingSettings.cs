using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace AssetStreaming
{
    public class UosCdnSettings : ScriptableObject
    {
        [SerializeField]
        public bool useLatest;
        
        [SerializeField]
        public bool syncWithDelete;

        private static UosCdnSettings m_settings;

        public static UosCdnSettings Settings
        {
            get
            {
                if (m_settings != null)
                {
                    return m_settings;
                }

                if (false == Directory.Exists(Parameters.k_AssetStreamingSettingsPathPrefix))
                {
                    Directory.CreateDirectory(Parameters.k_AssetStreamingSettingsPathPrefix);
                }

                m_settings = AssetDatabase.LoadAssetAtPath<UosCdnSettings>(Parameters.k_AssetStreamingSettingsPath);
                if (m_settings == null)
                {
                    m_settings = ScriptableObject.CreateInstance<UosCdnSettings>();
                    m_settings.useLatest = true;
                    m_settings.syncWithDelete = false;
                    AssetDatabase.CreateAsset(m_settings, Parameters.k_AssetStreamingSettingsPath);
                    AssetDatabase.SaveAssets();
                }

                return m_settings;

            }
        }
    }
}
