using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Passport.Sample.Scripts.Utils
{
    public class Utils
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void webCopy(string text);
#endif

        public static void Copy(string text)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            webCopy(text);
#else
            GUIUtility.systemCopyBuffer = text;
#endif
        }
    }
}