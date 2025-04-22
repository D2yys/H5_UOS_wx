using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 用来做Ipad 适配的脚本   ，这里注意适配时候是竖屏或是横屏需要手动调整 float tempRatio = (size.height / size.width) / ratio;  宽高的除以的顺序。 iphoneX比例也需要手动调整
public class IpadArea : MonoBehaviour
{
    Transform mTransform;

    public PlayMakerFSM Fsm;

    void Start()
    {
        mTransform = transform;
        if (mTransform == null) return;
        switch (GetScreenSizeType())
        {
            case ScreenSizeType.Size_4_3:
                //Debug.Log("4/3");
                Fsm.SendEvent("ipad");
                break;            
            case ScreenSizeType.Size_2436_1125:
               // Debug.Log("2436/1125");
                Fsm.SendEvent("iphoneX");
                break;
            //case ScreenSizeType.Size_16_10:
            //    Debug.Log("10/16");
            //    Fsm.SendEvent("iphoneX");
            //    break;
            //case ScreenSizeType.Size_18_9:
            //    Debug.Log("18/9");
            //    Fsm.SendEvent("iphoneX");
            //    break;
            //case ScreenSizeType.Size_16_9:
            //    Debug.Log("16/9");
            //    Fsm.SendEvent("iphone");
            //    break;
            default:
                //Debug.Log("default");
                Fsm.SendEvent("iphone");
                break;
        }
    }
#if UNITY_EDITOR
    void OnEnable()
    {
        Vector2 view = GetViewSize();
        Width = (int)view.x;
        Height = (int)view.y;
        Start();
    }
#endif


#if UNITY_EDITOR

    Vector2 GetViewSize()
    {
        var mouseWindow = UnityEditor.EditorWindow.mouseOverWindow;
        System.Reflection.Assembly a = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = a.GetType("UnityEditor.PlayModeView");
        Vector2 size = (Vector2)type.GetMethod("GetMainPlayModeViewTargetSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(mouseWindow, null);

        return size;
    }
#endif

    public enum ScreenSizeType
    {
        Size_16_9,
        Size_4_3,
        Size_2436_1125,
        Size_18_9,
        Size_16_10
    }

    public class ScreenStandard
    {
        public float width;
        public float height;
        public ScreenSizeType type;

        public ScreenStandard(ScreenSizeType type)
        {
            string temp = type.ToString();
            string[] temps = temp.Split('_');
            this.width = float.Parse(temps[1]);
            this.height = float.Parse(temps[2]);
            this.type = type;
        }
    }

    public static ScreenSizeType GetScreenSizeType ()
    {
        return GetNearestSize ().type;
    }

    public static int Height;
    public static int Width;

    public static ScreenStandard GetNearestSize ()
    {
#if UNITY_EDITOR
        //if (Height == 0)
        //{
        //    Height = Screen.height;
        //    Width = Screen.width;
        //}
        float ratio = Width * 1f / Height;
#else
        float ratio = Screen.width * 1f / Screen.height;
#endif

        float factor = 0;
        ScreenStandard result = defaultSize;
        foreach (ScreenStandard size in screenStandards) {
            float tempRatio = (size.height / size.width) / ratio;
            if (tempRatio > 1) {
                tempRatio = 1 / tempRatio;
            }

            if (tempRatio > factor) {
                factor = tempRatio;
                result = size;
            }
        }
        return result;
    }

    private static List<ScreenStandard> screenStandards = new List<ScreenStandard>(new ScreenStandard[] {
        new ScreenStandard (ScreenSizeType.Size_16_9),
        new ScreenStandard (ScreenSizeType.Size_4_3),
        new ScreenStandard (ScreenSizeType.Size_2436_1125),
        new ScreenStandard (ScreenSizeType.Size_16_10),
        new ScreenStandard (ScreenSizeType.Size_18_9)
    });
    private static ScreenStandard defaultSize = new ScreenStandard(ScreenSizeType.Size_16_9);
}