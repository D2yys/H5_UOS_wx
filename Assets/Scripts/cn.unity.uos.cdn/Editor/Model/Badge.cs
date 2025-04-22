using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetStreaming
{
    [Serializable]
    public class Badge
    {
        public string name;
        public string releaseid;
        public string releasenum;
        public string created;
    }
}