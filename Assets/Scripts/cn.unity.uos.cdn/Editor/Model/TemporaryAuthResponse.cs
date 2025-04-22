using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetStreaming
{
    [Serializable]
    public class TemporaryAuthResponse
    {
        public string tmpId;
        public string tmpKey;
        public string tmpToken;
        public long startTime;
        public long expiredTime;
    }
}