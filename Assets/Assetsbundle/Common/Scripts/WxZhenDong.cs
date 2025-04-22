
using UnityEngine;
//using WeChatWASM;

public class WxZhenDong : MonoBehaviour
{
    public bool close;

    public void zhendong()
    {

        if (Application.platform == RuntimePlatform.WebGLPlayer)

        {
            if (close == false)
            {
            bofang();
            }
        }

    }

       
  


   private void bofang()
    {

        //WX.VibrateShort(new VibrateShortOption() //调用震动方法
        //{
        //    success = (res) =>
        //    {
               
        //    },
        //    fail = (res) =>
        //    {
               
        //    },
        //    complete = (res) =>
        //    {
               
        //    }

        //});
    }

}
