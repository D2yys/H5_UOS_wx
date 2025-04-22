using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeDjs : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text;

    public int RedSecond;
    public int second;
    public PlayMakerFSM Main;
    public PlayMakerFSM Time;
    //增加的秒数

    public int addint;
    public void Timer()
    {
        InvokeRepeating("daojishi", 0, 1);
    }
    
    public void Stop()
    {
        CancelInvoke("daojishi");
    }

    // 总秒数的倒计时
    private void daojishi()
    {
        second = second - 1;
        text.color = Color.white;
        text.text = string.Format("{0:d2}:{1:d2}", (int)second / 60, (int)second % 60);

        switch(second)
        {
            case 30:
                Time.SendEvent("Time30");
             break;
            case 60:
                Time.SendEvent("Time60");
                break;
            case 180:
                Time.SendEvent("Time180");
                break;
            case 300:
                Time.SendEvent("Time300");
                break;

        }
        

        if (second <= RedSecond)
        {
            text.color = Color.red;
            if (second <= 0)
            {
                // 取消调用     
                Main.SendEvent("TimeIsUp");
                Stop();
            }
        }
    }

    //时间增加动画
    public void PlayAdd(int addTime)
    {
        //停止倒计时
        Stop();
        addint = second;
        addint = addint + addTime;
        InvokeRepeating("TimeAdd", 0, 0.03f);
    }
  

    private void TimeAdd()
    {
        second = second + 1;
        //Debug.Log(+second+"test"+addint+"");
        text.color = Color.white;
        text.text = string.Format("{0:d2}:{1:d2}", (int)second / 60, (int)second % 60);
        if(addint <=second)
        {
            CancelInvoke("TimeAdd");
            Timer();
        }
    }

    //冰冻情况下的时间增加，增加完后不继续倒计时
    public void PlayAddBD()
    {
        //停止倒计时
        Stop();
        addint = second;
        addint = addint + 10;
        InvokeRepeating("TimeAddBD", 0, 0.03f);

    }

    //冰冻模式的时钟道具
    private void TimeAddBD()
    {
        second = second + 1;
        //Debug.Log(+second + "test" + addint + "");
        text.color = Color.white;
        text.text = string.Format("{0:d2}:{1:d2}", (int)second / 60, (int)second % 60);
        if (addint <= second) {
            CancelInvoke("TimeAddBD");
        }
    }
}
