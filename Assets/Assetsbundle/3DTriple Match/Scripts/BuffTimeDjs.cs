using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffTimeDjs : MonoBehaviour
{
    // Start is called before the first frame update

   
    public Text text;
    public PlayMakerFSM Fsm;
    public PlayMakerFSM LightFsm;
    public PlayMakerFSM ClockFsm;
    public PlayMakerFSM DoubleFsm;
  
    //时间起始时间和结束时间需要进行数据存储
    public string liveStart;
    public string liveEnd;
    public string liveTest;

    public string LightStart;
    public string LightEnd;
    public string LightTest;

    public string ClockStart;
    public string ClockEnd;
    public string ClockTest;

    public string DoubleStart;
    public string DoubleEnd;
    public string DoubleTest;

    //无限生命Buff倒计时   

    //如果当前时间小于测试时间，说明时间被改变向前调过

    //调过后起始时间修改为当前时间，结束时间设为还剩的时间

    //还剩的时间=原起始时间 - 到测试的时间 

    //用还剩的时间加当前的时间，等于新的结束时间

    //用新起始时间和结束时间继续计时
    //数据保存
    private void SaveTime()
    {
        var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
        GameObject Manager = numLives.Value;

        Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "MySql").SendEvent("Updata");

    }


      //--------------------------------------体力Buff-----------------------------------用当前时间减结束时间 =剩余时间，剩余时间为0 重置起始和结束都为当前时间。 有剩余时间就用来倒计时。倒计时的时候检测当前时间是否小于测试时间，小于测试时间则为时间篡改
    public void LiveTime(int Minutes)
    {
        liveTest = DateTime.UtcNow.AddHours(+8).ToString();
        //没有值就初始起始时间和结束时间
        if (liveStart.Length == 0 || liveEnd.Length == 0 )
        {
            
            liveStart = DateTime.UtcNow.AddHours(+8).ToString();
            

            liveEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
           

            liveTest = DateTime.UtcNow.AddHours(+8).ToString();
            Debug.Log("buff时间重置");
            SaveTime();


        }
       
        //根据起始和结束获得剩余时间
        DateTime start = DateTime.Parse(liveStart);
        DateTime end = DateTime.Parse(liveEnd);
        DateTime test = DateTime.Parse(liveTest);
        
        DateTime newotime = DateTime.UtcNow.AddHours(+8);

        //获得剩buff余时间
        TimeSpan sc = end.Subtract(newotime);//结束时间减当前时间 = 剩余时间

        if (sc.TotalSeconds <= 0)
        {
            liveStart = DateTime.UtcNow.AddHours(+8).ToString();   //重置起始时间
            
            liveEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();//重置结束时间
            
            SaveTime();
        }
        else
        {
            liveEnd = end.AddMinutes(Minutes).ToString(); //还剩时间的话加上传入的时间
            Debug.Log("上传时间");
            SaveTime();
        }

        CancelInvoke("BuffLive");
        InvokeRepeating("BuffLive", 0, 1);
    }
    
    public void LiveStop()
    {
        CancelInvoke("BuffLive");
        Fsm.SendEvent("LiveBuffEnd");
    }

    private void BuffLive()
    {

        //获取起始，结束，test检测时间
        DateTime start = DateTime.Parse(liveStart);
        DateTime end = DateTime.Parse(liveEnd);
        DateTime test = DateTime.Parse(liveTest);
        //获得当前时间，
        DateTime newotime = DateTime.UtcNow.AddHours(+8);

        //获得剩buff余时间
        TimeSpan sc = end.Subtract(newotime);

        if (newotime < test)
        {

            //时间被篡改，先获得正常应该剩余的时长,老的时间到每秒记录的时间
            TimeSpan residue = end.Subtract(test);

            //把当前时间设为起始时间，结束时间设为起始时间+剩余时间
            liveStart = DateTime.UtcNow.AddHours(+8).ToString();

            liveEnd = DateTime.UtcNow.AddHours(+8).AddSeconds(residue.TotalSeconds).ToString();


            start = DateTime.Parse(liveStart);
            end = DateTime.Parse(liveEnd);

            //获得当前时间，
            newotime = DateTime.UtcNow.AddHours(+8);

            sc = end.Subtract(newotime);
            // Debug.Log("时间被篡改");
            SaveTime();
        }

        liveTest = DateTime.UtcNow.AddHours(+8).ToString();
        

        
        text.text = string.Format("{0:d2}:{1:d2}", (int)sc.TotalSeconds / 60, (int)sc.TotalSeconds % 60);

        if (sc.TotalSeconds <= 0)
        {
            // 取消调用                   
            LiveStop();
        }
        

    }

   // ---------------闪电Buff----------------------
    public void LightTime(int Minutes)
    {
        LightTest = DateTime.UtcNow.AddHours(+8).ToString();
        if (LightStart.Length == 0 || LightEnd.Length == 0 )
        {
            LightStart = DateTime.UtcNow.AddHours(+8).ToString();
            LightEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
            LightTest = DateTime.UtcNow.AddHours(+8).ToString();
            SaveTime();
        }
        
        DateTime start = DateTime.Parse(LightStart);
        DateTime end = DateTime.Parse(LightEnd);
        DateTime test = DateTime.Parse(LightTest);

        DateTime newotime = DateTime.UtcNow.AddHours(+8);
       
        TimeSpan sc = end.Subtract(newotime);

        if (sc.TotalSeconds <= 0)
        {
            LightStart = DateTime.UtcNow.AddHours(+8).ToString();
            LightEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
            SaveTime();
        }
        else
        {
            LightEnd = end.AddMinutes(Minutes).ToString();
            SaveTime();
        }

        CancelInvoke("BuffLight");
        InvokeRepeating("BuffLight", 0, 1);
    }
 
    public void LightStop()
    {
        CancelInvoke("BuffLight");
        LightFsm.SendEvent("LightEnd");
    }

    private void BuffLight()
    {
        
        DateTime start = DateTime.Parse(LightStart);
        DateTime end = DateTime.Parse(LightEnd);
        DateTime test = DateTime.Parse(LightTest);

        DateTime newotime = DateTime.UtcNow.AddHours(+8);
       
        TimeSpan sc = end.Subtract(newotime);

        if (newotime < test)
        {
            
            TimeSpan residue = end.Subtract(test);
           
            liveStart = DateTime.UtcNow.AddHours(+8).ToString();

            liveEnd = DateTime.UtcNow.AddHours(+8).AddSeconds(residue.TotalSeconds).ToString();


            start = DateTime.Parse(LightStart);
            end = DateTime.Parse(LightEnd);
          
            newotime = DateTime.UtcNow.AddHours(+8);

            sc = end.Subtract(newotime);
            SaveTime();
        }

        LightTest = DateTime.UtcNow.AddHours(+8).ToString();


        
        LightFsm.FsmVariables.GetFsmString("Time").Value = string.Format("{0:d2}:{1:d2}", (int)sc.TotalSeconds / 60, (int)sc.TotalSeconds % 60); 

        if (sc.TotalSeconds <= 0)
        {             
            LightStop();
        }

    }




    //---------------时钟buff---------------------------
    public void ClockTime(int Minutes)
    {
        ClockTest = DateTime.UtcNow.AddHours(+8).ToString();
        if (ClockStart.Length == 0 || ClockEnd.Length == 0 )
        {
            ClockStart = DateTime.UtcNow.AddHours(+8).ToString();
            ClockEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
            ClockTest = DateTime.UtcNow.AddHours(+8).ToString();
        }

        DateTime start = DateTime.Parse(ClockStart);
        DateTime end = DateTime.Parse(ClockEnd);
        DateTime test = DateTime.Parse(ClockTest);

        DateTime newotime = DateTime.UtcNow.AddHours(+8);

        TimeSpan sc = end.Subtract(newotime);

        if (sc.TotalSeconds <= 0)
        {
            ClockStart = DateTime.UtcNow.AddHours(+8).ToString();
            ClockEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
            SaveTime();
        }
        else
        {
            ClockEnd = end.AddMinutes(Minutes).ToString();
        }

        CancelInvoke("BufClock");
        InvokeRepeating("BuffClock", 0, 1);
    }

    public void ClockStop()
    {
        CancelInvoke("BuffClock");
        ClockFsm.SendEvent("ClockEnd");
    }

    private void BuffClock()
    {

        DateTime start = DateTime.Parse(ClockStart);
        DateTime end = DateTime.Parse(ClockEnd);
        DateTime test = DateTime.Parse(ClockTest);

        DateTime newotime = DateTime.UtcNow.AddHours(+8);

        TimeSpan sc = end.Subtract(newotime);

        if (newotime < test)
        {

            TimeSpan residue = end.Subtract(test);

            ClockStart = DateTime.UtcNow.AddHours(+8).ToString();

            ClockEnd = DateTime.UtcNow.AddHours(+8).AddSeconds(residue.TotalSeconds).ToString();


            start = DateTime.Parse(ClockStart);
            end = DateTime.Parse(ClockEnd);

            newotime = DateTime.UtcNow.AddHours(+8);

            sc = end.Subtract(newotime);
            SaveTime();
        }

        ClockTest = DateTime.UtcNow.AddHours(+8).ToString();


        ClockFsm.FsmVariables.GetFsmString("Time").Value = string.Format("{0:d2}:{1:d2}", (int)sc.TotalSeconds / 60, (int)sc.TotalSeconds % 60);
        

        if (sc.TotalSeconds <= 0)
        {
            ClockStop();
        }

    }



    //------------------Doublebuff-------------------
    public void DoubleTime(int Minutes)
    {
        DoubleTest = DateTime.UtcNow.AddHours(+8).ToString();
        if (DoubleStart.Length == 0 || DoubleEnd.Length == 0 )
        {
            DoubleStart = DateTime.UtcNow.AddHours(+8).ToString();
            DoubleEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
            DoubleTest = DateTime.UtcNow.AddHours(+8).ToString();
        }

        DateTime start = DateTime.Parse(DoubleStart);
        DateTime end = DateTime.Parse(DoubleEnd);
        DateTime test = DateTime.Parse(DoubleTest);

        DateTime newotime = DateTime.UtcNow.AddHours(+8);

        TimeSpan sc = end.Subtract(newotime);

        if (sc.TotalSeconds <= 0)
        {
            DoubleStart = DateTime.UtcNow.AddHours(+8).ToString();
            DoubleEnd = DateTime.UtcNow.AddHours(+8).AddMinutes(Minutes).ToString();
            SaveTime();
        }
        else
        {
            DoubleEnd = end.AddMinutes(Minutes).ToString();
            SaveTime();
        }

        CancelInvoke("DoubleClock");
        InvokeRepeating("DoubleClock", 0, 1);
    }

    public void DoubleStop()
    {
        CancelInvoke("DoubleClock");
        DoubleFsm.SendEvent("DoubleEnd");
    }

    private void DoubleClock()
    {

        DateTime start = DateTime.Parse(DoubleStart);
        DateTime end = DateTime.Parse(DoubleEnd);
        DateTime test = DateTime.Parse(DoubleTest);

        DateTime newotime = DateTime.UtcNow.AddHours(+8);

        TimeSpan sc = end.Subtract(newotime);

        if (newotime < test)
        {

            TimeSpan residue = end.Subtract(test);

            DoubleStart = DateTime.UtcNow.AddHours(+8).ToString();

            DoubleEnd = DateTime.UtcNow.AddHours(+8).AddSeconds(residue.TotalSeconds).ToString();


            start = DateTime.Parse(DoubleStart);
            end = DateTime.Parse(DoubleEnd);

            newotime = DateTime.UtcNow.AddHours(+8);

            sc = end.Subtract(newotime);
            SaveTime();
        }

        DoubleTest = DateTime.UtcNow.AddHours(+8).ToString();


        DoubleFsm.FsmVariables.GetFsmString("Time").Value = string.Format("{0:d2}:{1:d2}", (int)sc.TotalSeconds / 60, (int)sc.TotalSeconds % 60);
        

        if (sc.TotalSeconds <= 0)
        {
            DoubleStop();
        }

    }
   
}
