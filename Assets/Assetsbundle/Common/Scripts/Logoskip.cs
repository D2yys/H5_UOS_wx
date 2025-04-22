using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

[Preserve]
public static class Logoskip
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void BeforeSplashScreen()
    {
#if UNITY_WEBGL
        Application.focusChanged += ApplicationFocusChanged;
#else
        Task.Run(() => SplashScreen.Stop(SplashScreen.StopBehavior.FadeOut));
#endif
    }

#if UNITY_WEBGL
    private static void ApplicationFocusChanged(bool obj)
    {
        Application.focusChanged -= ApplicationFocusChanged;
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
    }
#endif
}