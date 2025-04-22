using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTime : MonoBehaviour
{

    [SerializeField] Text text;
 
    public PlayMakerFSM Fsm;
    public DateTime EndTime;
    int pdbool;  //首次运行bool
    public int TimeHours;//倒计时的小时数量
    public bool TimeEnd;
    TimeSpan span;
    public string Savekey;
    public string EndString; //最终结束时间

     void Awake()
    {
        PlayTime();
    }

    public void PlayTime()//显示时分秒 取值是默认值则判断是第一次取值    bool 0是首次，1不是首次
    {

        if (PlayerPrefs.GetString(Savekey, EndString) == "" && PlayerPrefs.GetInt(Savekey, pdbool) == 0)
        {
            PlayerPrefs.SetInt(Savekey, 1);
            DateTime a = DateTime.UtcNow.AddHours(+8).AddHours(TimeHours);//前时间+24小时
            EndString = a.ToString();
            PlayerPrefs.SetString(Savekey, EndString); //保存结束时间
            span = DateTime.UtcNow.AddHours(+8).Subtract(DateTime.Parse(EndString)).Duration(); //获得总时长
           
            InvokeRepeating("ShopDjs", 0, 1);
        }
        else
        {
            EndString = PlayerPrefs.GetString(Savekey, "defaultValue");
            DateTime nowTime = DateTime.UtcNow.AddHours(+8);
             span = nowTime.Subtract(DateTime.Parse(EndString)).Duration(); //获得总时长
                                                                            //
            if (span.TotalSeconds > 0)
            {
                InvokeRepeating("ShopDjs", 0, 1);
            }
            else
            {
                TimeEnd = true;
                Fsm.SendEvent("End");
               
            }

        }
    }

    public void Stop()
    {
        CancelInvoke("ShopDjs");
        
    }


    private void ShopDjs()
    {      

        span = DateTime.UtcNow.AddHours(+8).Subtract(DateTime.Parse(EndString)).Duration();
        string sj = string.Format("{0:d2}:{1:d2}:{2:d2}", span.Hours, span.Minutes, span.Seconds);
        text.text = "礼包失效时间" + sj;     
        if (span.TotalSeconds <= 0)
        {
            Stop();
            TimeEnd = true;
            Fsm.SendEvent("End");
        }
    }
}
